using MusicManagementCore.Constant;
using MusicManagementCore.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MusicManagementCore.Util
{
    public class TableOfContentsUtil
    {
        public static string ReadVersion(string filename)
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

        public static T ReadFromFile<T>(string filename)
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

        public static TableOfContentsV2 MigrateV1ToV2File(string filename)
        {
            var tocV1 = ReadFromFile<TableOfContentsV1>(filename);
            var tocV2 = ConvertToV2(tocV1);
            File.Copy(filename, filename + "_v1bak");
            JsonWriter.WriteToFilename(filename, tocV2);
            return tocV2;
        }

        public static TableOfContentsV2 ConvertToV2(TableOfContentsV1 tocV1)
        {
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
                trackV2.MetaHash = HashTags(trackV2);
                tocV2.TrackList.Add(trackV2);
            });
            tocV2.TrackList.Sort((t1, t2) => t1.TrackNumber.CompareTo(t2.TrackNumber));

            return tocV2;
        }

        public static string HashTags(TrackV2 track)
        {
            var hashSource = track.Artist + track.Album + track.Genre + track.Year
                + track.TrackNumber + track.TrackTitle;

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashSource));

            return Convert.ToHexString(bytes);
        }
    }
}
