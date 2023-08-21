using CommandLine;
using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using MusicManagementCore.Model;
using MusicManagementCore.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CreateToc
{
    class CreateToc
    {
        public class Options
        {
            [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
            public required string Config { get; set; }
        }

        static Dictionary<string, TableOfContentsV2> records = new Dictionary<string, TableOfContentsV2>();

        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var convertMusic = new CreateToc(parserResult.Value);
            return convertMusic.Run();
        }


        private readonly Configuration _config;

        public CreateToc(Options options)
        {
            _config = new Configuration(options.Config);
        }

        public int Run()
        {
            var fileFinder = new MusicMgmtFileFinder(_config.InputConfig, _config.FilenameEncodingConfig);
            fileFinder.EnterDirectory += EnterDirectory;
            fileFinder.LeaveDirectory += LeaveDirectory;
            fileFinder.FoundAudioFile += FoundAudioFile;

            fileFinder.Scan();
            return 0;
        }

        private string ReplaceCodeStrings(string value)
        {
            var codes = _config.FilenameEncodingConfig.CharacterReplacements;
            codes.ForEach(c => value = value.Replace(c.Replacement, c.Character));
            return value;
        }

        private string RemoveCodeStrings(string value)
        {
            var codes = _config.FilenameEncodingConfig.CharacterReplacements;
            codes.ForEach(c => value = value.Replace(c.Replacement, " "));
            return value.Replace(".", "");
        }

        private void FoundAudioFile(object _, AudioFileEvent e)
        {
            if (records.ContainsKey(e.AudioFile.Directory))
            {
                var toc = records.First(item => item.Key == e.AudioFile.Directory);
                toc.Value.TrackList.Add(TrackFromAudioFile(e.AudioFile));
            }
        }

        private TrackV2 TrackFromAudioFile(AudioFile audioFile)
        {
            var track = new TrackV2
            {
                Artist = ReplaceCodeStrings(audioFile.MetaData.Artist),
                Album = ReplaceCodeStrings(audioFile.MetaData.Album),
                Genre = audioFile.MetaData.Genre,
                Year = audioFile.MetaData.Year,
                TrackNumber = audioFile.MetaData.TrackNumber,
                TrackTitle = ReplaceCodeStrings(audioFile.MetaData.TrackTitle)
            };

            var filename = new AudioFilename
            {
                LongName = audioFile.Filename,
                ShortName = ShortFilename(audioFile)
            };

            track.Filename = filename;

            return track;
        }

        private string ShortFilename(AudioFile audioFile)
        {
            var fileInfo = new FileInfo(audioFile.Filename);
            var metaData = audioFile.MetaData;
            return $"{metaData.TrackNumber} - {RemoveCodeStrings(metaData.TrackTitle)}{fileInfo.Extension}";
        }

        private static void EnterDirectory(object _, DirectoryEvent e)
        {
            if (!File.Exists(Path.Combine(e.DirectoryPath, StandardFilename.TableOfContents)))
            {
                records.Add(e.DirectoryPath, new TableOfContentsV2());
            }
            else
            {
                Console.WriteLine($"'{e.DirectoryPath}' already contains a table of contents file.");
            }
        }

        private static void LeaveDirectory(object _, DirectoryEvent e)
        {
            if (records.ContainsKey(e.DirectoryPath))
            {
                var toc = records.First(item => item.Key == e.DirectoryPath);
                FinalizeRecord(e.DirectoryPath, toc.Value);
                records.Remove(e.DirectoryPath);
            }
        }

        private static void FinalizeRecord(string directory, TableOfContentsV2 toc)
        {
            if (toc.TrackList.Count == 0)
            {
                return;
            }

            var distinctArtists = toc.TrackList.DistinctBy(track => track.Artist);
            var isCompilation = distinctArtists.Count() > 1;
            toc.TrackList.ForEach(track => track.IsCompilation = isCompilation);
            toc.TrackList.Sort((t1, t2) => t1.TrackNumber.CompareTo(t2.TrackNumber));

            JsonWriter.WriteToDirectory(directory, toc);
            toc.TrackList.ForEach(track => RenameFile(directory, track));
        }

        private static void RenameFile(string directory, TrackV2 track)
        {
            File.Move(
                Path.Combine(directory, track.Filename.LongName),
                Path.Combine(directory, track.Filename.ShortName));
        }
    }
}
