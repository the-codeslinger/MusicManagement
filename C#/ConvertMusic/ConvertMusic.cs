using CommandLine;
using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using System.Diagnostics;
using System.Text.Json;

namespace ConvertMusic
{
    class ConvertMusic
    {
        public class Options
        {
            [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
            public string Config { get; set; }

            [Option('f', "format", HelpText = "The converter to use. This identifies the converter in the 'Output' section of the configuration file.")]
            public string Format { get; set; }
        }

        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var convertMusic = new ConvertMusic(parserResult.Value);
            return convertMusic.Run();
        }


        private const string CompilationArtistName = "Compilation";

        private Options _options;
        private Configuration _config;
        private Converter _converter;

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

            var toc = ReadTableOfContents(e.TableOfContentsFile);
            var directory = Path.GetDirectoryName(e.TableOfContentsFile);

            toc.TrackList.ForEach(track => HandleTrack(directory!, track));
        }

        private string MakeDestinationFilename(TrackV2 track)
        {
            var formatted = _config.OutputConfig.Format
                .Replace(MetaTagName.Artist, track.IsCompilation ? CompilationArtistName : track.Artist)
                .Replace(MetaTagName.Album, track.Album)
                .Replace(MetaTagName.Genre, track.Genre)
                .Replace(MetaTagName.Year, track.Year)
                .Replace(MetaTagName.TrackNumber, track.TrackNumber)
                .Replace(MetaTagName.Title, track.TrackTitle);
            return Path.Combine(_converter.Output.Path, formatted + "." + _converter.Type.ToLower());
        }

        private void MakeDestinationFolder(string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if (null != directory && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void HandleTrack(string directory, TrackV2 track)
        {
            var source = Path.Combine(directory, track.Filename.ShortName);
            if (!File.Exists(source))
            {
                throw new FileNotFoundException($"Audio file '{source}' does not exist.");
            }

            var destination = MakeDestinationFilename(track);
            MakeDestinationFolder(destination);

            if (!File.Exists(destination))
            {
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

        private void WriteAudioTags(string destinationFile, string coverArt, TrackV2 track)
        {

        }

        private string ReadToCVersion(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            try
            {
                var versionElement = json.RootElement.GetProperty("version");
                return versionElement.GetString() ?? ToCVersion.V1;
            }
            catch (KeyNotFoundException _)
            {
                // If "version" does not exist, we have a V1 at our hands.
                return ToCVersion.V1;
            }
        }

        private void ConvertV1ToV2(string filename)
        {

        }

        private static TableOfContentsV2 ReadTableOfContents(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            return json.RootElement.Deserialize<TableOfContentsV2>();
        }
    }
}