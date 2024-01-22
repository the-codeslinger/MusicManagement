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
using MusicManagementCore.Service;

namespace CreateToc
{
    class CreateToc
    {
        private readonly Dictionary<string, TableOfContentsV2> _records = new Dictionary<string, TableOfContentsV2>();
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

        private void EnterDirectory(object _, DirectoryEvent e)
        {
            var tocFilename = Path.Combine(e.DirectoryPath, StandardFilename.TableOfContents);
            if (!File.Exists(tocFilename)) {
                _records.Add(e.DirectoryPath, new TableOfContentsV2());
            }
            else {
                var version = TableOfContentsUtil.ReadVersion(tocFilename);
                if (ToCVersion.V1 == version) {
                    TableOfContentsUtil.MigrateV1ToV2File(tocFilename);
                    Console.WriteLine($"'{e.DirectoryPath}' V1 table of contents file migrated to V2.");
                }
                else {
                    var toc = TableOfContentsUtil.ReadFromFile<TableOfContentsV2>(tocFilename);
                    var coverHashUpdated = UpdateCoverHashIfMissing(e.DirectoryPath, toc);
                    var metaHashUpdated = UpdateMetaHashIfMissing(toc);

                    if (coverHashUpdated || metaHashUpdated) {
                        JsonWriter.WriteToDirectory(e.DirectoryPath, toc);
                        Console.WriteLine($"'{e.DirectoryPath}' data hashes in updated.");
                    }
                    else {
                        Console.WriteLine($"'{e.DirectoryPath}' already contains a table of contents file.");
                    }
                }
            }
        }

        private void LeaveDirectory(object _, DirectoryEvent e)
        {
            if (_records.ContainsKey(e.DirectoryPath)) {
                var toc = _records.First(item => item.Key == e.DirectoryPath);
                FinalizeRecord(e.DirectoryPath, toc.Value);
                _records.Remove(e.DirectoryPath);

                Console.WriteLine($"'{e.DirectoryPath}' is the proud owner of a new table of contents file.");
            }
        }

        private void FoundAudioFile(object _, AudioFileEvent e)
        {
            if (_records.ContainsKey(e.AudioFile.Directory)) {
                var toc = _records.First(item => item.Key == e.AudioFile.Directory);
                toc.Value.TrackList.Add(TrackFromAudioFile(e.AudioFile));
            }
        }

        private string ReplaceCodeStrings(string value)
        {
            return _config.FilenameEncodingConfig.ReplaceCodeStrings(value);
        }

        private string RemoveCodeStrings(string value)
        {
            return _config.FilenameEncodingConfig.RemoveCodeStrings(value);
        }

        private TrackV2 TrackFromAudioFile(AudioFile audioFile)
        {
            var track = new TrackV2 {
                Artist = ReplaceCodeStrings(audioFile.MetaData.Artist),
                Album = ReplaceCodeStrings(audioFile.MetaData.Album),
                Genre = ReplaceCodeStrings(audioFile.MetaData.Genre),
                Year = audioFile.MetaData.Year,
                TrackNumber = audioFile.MetaData.TrackNumber,
                TrackTitle = ReplaceCodeStrings(audioFile.MetaData.TrackTitle)
            };

            var filename = new AudioFilename {
                LongName = audioFile.Filename,
                ShortName = ShortFilename(audioFile)
            };

            track.Filename = filename;
            track.UpdateHash();

            return track;
        }

        private string ShortFilename(AudioFile audioFile)
        {
            var fileInfo = new FileInfo(audioFile.Filename);
            var metaData = audioFile.MetaData;
            return $"{metaData.TrackNumber} - {RemoveCodeStrings(metaData.TrackTitle)}{fileInfo.Extension}";
        }

        private static void FinalizeRecord(string directory, TableOfContentsV2 toc)
        {
            if (toc.TrackList.Count == 0) {
                return;
            }

            var distinctArtists = toc.TrackList.DistinctBy(track => track.Artist);
            var isCompilation = distinctArtists.Count() > 1;
            toc.TrackList.ForEach(track => track.IsCompilation = isCompilation);
            toc.TrackList.Sort((t1, t2) => t1.TrackNumber.CompareTo(t2.TrackNumber));
            toc.UpdateHash(directory);

            JsonWriter.WriteToDirectory(directory, toc);
            toc.TrackList.ForEach(track => RenameFile(directory, track));
        }

        private static void RenameFile(string directory, TrackV2 track)
        {
            File.Move(
                Path.Combine(directory, track.Filename.LongName),
                Path.Combine(directory, track.Filename.ShortName));
        }

        private static bool UpdateCoverHashIfMissing(string directory, TableOfContentsV2 toc)
        {
            if (String.IsNullOrEmpty(toc.CoverHash)) {
                toc.UpdateHash(directory);
                return true;
            }
            else {
                return false;
            }
        }

        private static bool UpdateMetaHashIfMissing(TableOfContentsV2 toc)
        {
            var updated = false;
            toc.TrackList.ForEach((track) => {
                if (String.IsNullOrEmpty(track.MetaHash)) {
                    track.UpdateHash();
                    updated = true;
                }
            });
            return updated;
        }
    }
}
