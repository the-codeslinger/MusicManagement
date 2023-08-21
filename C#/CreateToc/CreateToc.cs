using CommandLine;
using MusicManagementCore;
using MusicManagementCore.Config;
using MusicManagementCore.Constant;
using MusicManagementCore.Event;
using MusicManagementCore.Json;
using MusicManagementCore.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CreateToc
{
    class CreateToc
    {
        public class Options
        {
            [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
            public string Config { get; set; }
        }

        static Dictionary<string, TableOfContentsV2> records = new Dictionary<string, TableOfContentsV2>();

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options>(args).MapResult(Run, _ => 1);
        }

        static int Run(Options options)
        {
            var config = new Configuration(options.Config);

            var fileFinder = new MusicMgmtFileFinder(config.InputConfig, config.FilenameEncodingConfig);
            fileFinder.EnterDirectory += EnterDirectory;
            fileFinder.LeaveDirectory += LeaveDirectory;
            fileFinder.FoundAudioFile += FoundAudioFile;

            fileFinder.Scan();
            return 0;
        }

        static void EnterDirectory(object _, DirectoryEvent e)
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

        static void LeaveDirectory(object _, DirectoryEvent e)
        {
            if (records.ContainsKey(e.DirectoryPath))
            {
                var toc = records.First(item => item.Key == e.DirectoryPath);
                FinalizeRecord(e.DirectoryPath, toc.Value);
                records.Remove(e.DirectoryPath);
            }
        }

        static void FoundAudioFile(object _, AudioFileEvent e)
        {
            if (records.ContainsKey(e.AudioFile.Directory))
            {
                var toc = records.First(item => item.Key == e.AudioFile.Directory);
                toc.Value.TrackList.Add(TrackFromAudioFile(e.AudioFile));
            }
        }

        static void FinalizeRecord(string directory, TableOfContentsV2 toc)
        {
            if (toc.TrackList.Count == 0)
            {
                return;
            }

            var distinctArtists = toc.TrackList.DistinctBy(track => track.Artist);
            var isCompilation = distinctArtists.Count() > 1;
            toc.TrackList.ForEach(track  => track.IsCompilation = isCompilation);
            toc.TrackList.Sort((t1, t2) => t1.TrackNumber.CompareTo(t2.TrackNumber));

            WriteJson(directory, toc);
            toc.TrackList.ForEach(track => RenameFile(directory, track));
        }

        static TrackV2 TrackFromAudioFile(AudioFile audioFile)
        {
            var track = new TrackV2
            {
                Artist = audioFile.MetaData.Artist,
                Album = audioFile.MetaData.Album,
                Genre = audioFile.MetaData.Genre,
                Year = audioFile.MetaData.Year,
                TrackNumber = audioFile.MetaData.TrackNumber,
                TrackTitle = audioFile.MetaData.TrackTitle
            };

            var filename = new AudioFilename
            {
                LongName = audioFile.Filename,
                ShortName = ShortFilename(audioFile)
            };

            track.Filename = filename;

            return track;
        }

        static string ShortFilename(AudioFile audioFile)
        {
            var fileInfo = new FileInfo(audioFile.Filename);
            var metaData = audioFile.MetaData;
            return $"{metaData.TrackNumber} - {metaData.TrackTitle}{fileInfo.Extension}";
        }

        static void WriteJson(string directory, TableOfContentsV2 toc)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(toc, options);
            File.WriteAllText(Path.Combine(directory, StandardFilename.TableOfContents),
                jsonString);
        }

        static void RenameFile(string directory, TrackV2 track)
        {
            File.Move(
                Path.Combine(directory, track.Filename.LongName),
                Path.Combine(directory, track.Filename.ShortName));
        }
    }
}
