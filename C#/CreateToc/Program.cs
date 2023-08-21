using CommandLine;
using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using MusicManagementCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CreateToc
{
    class Program
    {
        public class Options
        {
            [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
            public string Config { get; set; }
        }


        static Dictionary<String, TableOfContentsV2> records = new Dictionary<String, TableOfContentsV2>();

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args).MapResult(o => Run(o), _ => 1);
        }

        static int Run(Options options)
        {
            var config = new Configuration(options.Config);

            /*
            var filename = "C:\\Users\\lober\\OneDrive\\Code\\C#\\MusicManagement\\CreateToc\\Config.json";

            var oldTocName = "E:\\Music\\ToC Sample.json";
            using var oldTocStream = new FileStream(oldTocName, FileMode.Open, FileAccess.Read);
            using var oldTocJson = JsonDocument.Parse(oldTocStream);
            var oldToc = oldTocJson.Deserialize<AlbumToC>();

            var newTocName = "E:\\Music\\ToC New.json";
            using var newTocStream = new FileStream(newTocName, FileMode.Open, FileAccess.Read);
            using var newTocJson = JsonDocument.Parse(newTocStream);
            var newToc = newTocJson.Deserialize<CompilationToC>();
            */

            var texasRanger = new AudioFileFinder(config.InputConfig, config.FilenameEncodingConfig);
            texasRanger.EnterDirectory += EnterDirectory;
            texasRanger.LeaveDirectory += LeaveDirectory;
            texasRanger.FoundAudioFile += FoundAudioFile;

            texasRanger.Walk();
            return 0;
        }

        static void EnterDirectory(object _, DirectoryEvent e)
        {
            records.Add(e.DirectoryPath, new TableOfContentsV2());
        }

        static void LeaveDirectory(object _, DirectoryEvent e)
        {
            var toc = records.First(item => item.Key == e.DirectoryPath);
            PrintAlbum(toc.Value);
            records.Remove(e.DirectoryPath);
        }

        static void FoundAudioFile(object _, AudioFileEvent e)
        {
            var toc = records.First(item => item.Key == e.AudioFile.Directory);
            toc.Value.TrackList.Add(TrackFromAudioFile(e.AudioFile));
        }

        static void PrintAlbum(TableOfContentsV2 toc)
        {
            if (toc.TrackList.Count == 0) return;

            Console.WriteLine("=== START RECORD ===");

            foreach (TrackV2 track in toc.TrackList)
            {
                Console.WriteLine($" * {track.ToString()}");
            }

            Console.WriteLine("=== END RECORD ===");
        }

        static TrackV2 TrackFromAudioFile(AudioFile audioFile)
        {
            var track = new TrackV2();
            track.Artist = audioFile.MetaData.Artist;
            track.Album = audioFile.MetaData.Album;
            track.Genre = audioFile.MetaData.Genre;
            track.Year = audioFile.MetaData.Year;
            track.TrackNumber = audioFile.MetaData.TrackNumber;
            track.TrackTitle = audioFile.MetaData.TrackTitle;

            var filename = new AudioFilename();
            filename.LongName = audioFile.Filename;
            filename.ShortName = ShortFilename(audioFile.MetaData);

            return track;
        }

        static string ShortFilename(MetaData metaData)
        {
            return "";
        }
    }
}
