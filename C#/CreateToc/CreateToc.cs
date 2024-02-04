using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Event;
using MusicManagementCore.Service;
using MusicManagementCore.Util;

namespace CreateToc
{
    internal class CreateToc(Options options)
    {
        private readonly Dictionary<string, TableOfContentsV2> _records = new();
        private readonly MusicManagementConfig _config = new(options.Config);

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
                        Console.WriteLine($"'{e.DirectoryPath}' data hashes updated.");
                    }
                    else {
                        Console.WriteLine($"'{e.DirectoryPath}' already contains a table of contents file.");
                    }
                }
            }
        }

        private void LeaveDirectory(object _, DirectoryEvent e)
        {
            if (!_records.ContainsKey(e.DirectoryPath)) return;
            
            var toc = _records.First(item => item.Key == e.DirectoryPath);
            FinalizeRecord(e.DirectoryPath, toc.Value);
            _records.Remove(e.DirectoryPath);

            Console.WriteLine($"'{e.DirectoryPath}' is the proud owner of a new table of contents file.");
        }

        private void FoundAudioFile(object _, AudioFileEvent e)
        {
            if (!_records.ContainsKey(e.UncompressedFile.Directory)) return;
            
            var toc = _records.First(item => item.Key == e.UncompressedFile.Directory);
            toc.Value.TrackList.Add(TrackFromAudioFile(e.UncompressedFile));
        }

        private string ReplaceCodeStrings(string value)
        {
            return _config.FilenameEncodingConfig.ReplaceCodeStrings(value);
        }

        private string RemoveCodeStrings(string value)
        {
            return _config.FilenameEncodingConfig.RemoveCodeStrings(value);
        }

        private TrackV2 TrackFromAudioFile(UncompressedFile uncompressedFile)
        {
            var track = new TrackV2 {
                Artist = ReplaceCodeStrings(uncompressedFile.MetaData.Artist),
                Album = ReplaceCodeStrings(uncompressedFile.MetaData.Album),
                Genre = ReplaceCodeStrings(uncompressedFile.MetaData.Genre),
                Year = uncompressedFile.MetaData.Year,
                TrackNumber = uncompressedFile.MetaData.TrackNumber,
                TrackTitle = ReplaceCodeStrings(uncompressedFile.MetaData.TrackTitle)
            };

            var filename = new AudioFilename {
                LongName = uncompressedFile.Filename,
                ShortName = ShortFilename(uncompressedFile)
            };

            track.Filename = filename;
            track.UpdateHash();

            return track;
        }

        private string ShortFilename(UncompressedFile uncompressedFile)
        {
            var fileInfo = new FileInfo(uncompressedFile.Filename);
            var metaData = uncompressedFile.MetaData;
            return $"{metaData.TrackNumber} - {RemoveCodeStrings(metaData.TrackTitle)}{fileInfo.Extension}";
        }

        private static void FinalizeRecord(string directory, TableOfContentsV2 toc)
        {
            if (toc.TrackList.Count == 0) return;

            var distinctArtists = toc.TrackList.DistinctBy(track => track.Artist);
            var isCompilation = distinctArtists.Count() > 1;
            toc.TrackList.ForEach(track => track.IsCompilation = isCompilation);
            toc.TrackList.Sort((t1, t2) => string.Compare(t1.TrackNumber, t2.TrackNumber, StringComparison.Ordinal));
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
            if (!string.IsNullOrEmpty(toc.CoverHash)) return false;
            toc.UpdateHash(directory);
            return true;

        }

        private static bool UpdateMetaHashIfMissing(TableOfContentsV2 toc)
        {
            var updated = false;
            toc.TrackList.ForEach(track =>
            {
                if (!string.IsNullOrEmpty(track.MetaHash)) return;
                track.UpdateHash();
                updated = true;
            });
            return updated;
        }
    }
}
