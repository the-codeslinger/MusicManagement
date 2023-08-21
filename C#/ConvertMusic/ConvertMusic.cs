using CommandLine;
using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using MusicManagementCore.Util;
using System.Diagnostics;
using System.Text.Json;

namespace ConvertMusic
{
    class ConvertMusic
    {
        public class Options
        {
            [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
            public required string Config { get; set; }

            [Option('f', "format", HelpText = "The converter to use. This identifies the converter in the 'Output' section of the configuration file.")]
            public required string Format { get; set; }
        }

        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var convertMusic = new ConvertMusic(parserResult.Value);
            return convertMusic.Run();
        }


        private const string CompilationArtistName = "Compilation";

        private readonly Options _options;
        private readonly Configuration _config;
        private readonly Converter _converter;

        public ConvertMusic(Options options)
        {
            _options = options;
            _config = new Configuration(_options.Config);

            var converter = _config.OutputConfig.Converters.Find(
                conv => conv.Type.ToLower() == _options.Format.ToLower());
            if (null == converter)
            {
                throw new ArgumentException($"No converter format found for '{_options.Format}'.");
            }
            _converter = converter;
        }

        public int Run()
        {
            var fileFinder = new MusicMgmtFileFinder(_config.InputConfig, _config.FilenameEncodingConfig);
            fileFinder.FoundTableOfContentsFile += FoundTableOfContentsFile;

            fileFinder.Scan();
            return 0;
        }

        private void FoundTableOfContentsFile(object _, TableOfContentsFileEvent e)
        {
            var version = ReadToCVersion(e.TableOfContentsFile);
            if (ToCVersion.V1 == version)
            {
                ConvertV1ToV2(e.TableOfContentsFile);
            }

            var toc = ReadTableOfContents<TableOfContentsV2>(e.TableOfContentsFile);
            var directory = Path.GetDirectoryName(e.TableOfContentsFile);

            toc.TrackList.ForEach(track => HandleTrack(directory!, track));
        }

        private string MakeDestinationFilename(TrackV2 track)
        {
            var formatted = _config.OutputConfig.Format
                .Replace(MetaTagName.Artist, RemoveCodeStrings(track.IsCompilation ? CompilationArtistName : track.Artist))
                .Replace(MetaTagName.Album, RemoveCodeStrings(track.Album))
                .Replace(MetaTagName.Genre, track.Genre)
                .Replace(MetaTagName.Year, track.Year)
                .Replace(MetaTagName.TrackNumber, track.TrackNumber)
                .Replace(MetaTagName.Title, RemoveCodeStrings(track.TrackTitle));
            return Path.Combine(_converter.Output.Path, formatted + "." + _converter.Type.ToLower());
        }

        private void HandleTrack(string directory, TrackV2 track)
        {
            var source = Path.Combine(directory, track.Filename.ShortName);
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Audio file '{source}' does not exist.");
            }

            var destination = MakeDestinationFilename(track);
            if (!File.Exists(destination))
            {
                MakeDestinationFolder(destination);

                Compress(source, destination);
                WriteAudioTags(destination, Path.Combine(directory, StandardFilename.AlbumCover), track);
            }
        }

        private void Compress(string source, string destination)
        {
            var args = _converter.Command.Args.ConvertAll(arg =>
            {
                if (arg == ConverterArgs.Input)
                {
                    return source;
                }
                else if (arg == ConverterArgs.Output)
                {
                    return destination;
                }
                else
                {
                    return arg;
                }
            });
            var process = Process.Start(_converter.Command.Bin, args);
            process.WaitForExit();
        }

        private string RemoveCodeStrings(string value)
        {
            var codes = _config.FilenameEncodingConfig.CharacterReplacements;
            codes.ForEach(c => value = value.Replace(c.Character, " "));
            return value.Replace(".", "");
        }

        private static void WriteAudioTags(string audioFile, string coverArt, TrackV2 track)
        {
            var file = TagLib.File.Create(audioFile);

            file.Tag.Album = track.Album;
            file.Tag.Title = track.TrackTitle;
            file.Tag.Track = Convert.ToUInt32(track.TrackNumber);
            file.Tag.Year = Convert.ToUInt32(track.Year);
            file.Tag.Genres = new string[] { track.Genre };
            file.Tag.Performers = new string[] { track.Artist };
            file.Tag.AlbumArtists = new string[] { track.IsCompilation ? CompilationArtistName : track.Artist };

            file.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(coverArt) {
                    Type = TagLib.PictureType.FrontCover,
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                }
            };

            file.Save();
        }

        private static string ReadToCVersion(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            try
            {
                var versionElement = json.RootElement.GetProperty("version");
                return versionElement.GetString() ?? ToCVersion.V1;
            }
            catch (KeyNotFoundException)
            {
                // If "version" does not exist, we have a V1 at our hands.
                return ToCVersion.V1;
            }
        }

        private static void ConvertV1ToV2(string filename)
        {
            var tocV1 = ReadTableOfContents<TableOfContentsV1>(filename);
            var tocV2 = new TableOfContentsV2();

            tocV1.TrackList.ForEach(trackV1 =>
            {
                var trackV2 = new TrackV2
                {
                    Artist = tocV1.Artist,
                    Album = tocV1.Album,
                    Genre = tocV1.Genre,
                    Year = tocV1.Year,

                    TrackNumber = trackV1.Number,
                    TrackTitle = trackV1.Title,

                    Filename = trackV1.Filename
                };
                tocV2.TrackList.Add(trackV2);
            });

            JsonWriter.WriteToFilename(filename, tocV2);
        }

        private static void MakeDestinationFolder(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if (null != directory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static T ReadTableOfContents<T>(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            var toc = json.RootElement.Deserialize<T>();
            if (null == toc)
            {
                throw new InvalidDataException($"File '{filename}' is not a valid table of contents.");
            }
            return toc;
        }
    }
}