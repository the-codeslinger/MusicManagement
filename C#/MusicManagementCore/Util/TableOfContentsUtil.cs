using System;
using MusicManagementCore.Constant;
using MusicManagementCore.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MusicManagementCore.Util
{
    /// <summary>
    /// A helper class to group a few useful methods in one place that are required in several
    /// locations.
    /// </summary>
    public static class TableOfContentsUtil
    {
        /// <summary>
        /// Read the 'version' property of the given table of contents file.
        /// </summary>
        /// <param name="filename">The filename of the table of contents file. The file will
        /// not be parsed into any of the classes located in the <cref>MusicManagementCore.Json</cref>
        /// namespace.</param>
        /// <returns>The version string if found or <cref>ToCVersion.V1</cref> otherwise.</returns>
        public static string ReadVersion(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            try {
                var versionElement = json.RootElement.GetProperty(JsonPropertyName.Version);
                return versionElement.GetString() ?? ToCVersion.V1;
            }
            catch (KeyNotFoundException) {
                // If "version" does not exist, we have a V1 at our hands.
                return ToCVersion.V1;
            }
        }

        /// <summary>
        /// Parse the given table of contents file into <cref>TableOfContentsV1</cref> or
        /// <cref>TableOfContentsV2</cref>.
        /// </summary>
        /// <typeparam name="T">One of <cref>TableOfContentsV1</cref> or <cref>TableOfContentsV2</cref>.</typeparam>
        /// <param name="filename">The filename of the table of contents file.</param>
        /// <returns>The table of contents file parsed into the desired C# class instance.</returns>
        /// <exception cref="InvalidDataException">The file does not contain a valid table of 
        /// contents file.</exception>
        public static T ReadFromFile<T>(string filename)
        {
            using var stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using var json = JsonDocument.Parse(stream);

            var toc = json.RootElement.Deserialize<T>();
            if (null == toc) {
                throw new InvalidDataException($"File '{filename}' is not a valid table of contents.");
            }
            return toc;
        }

        /// <summary>
        /// Migrate the given V1 table of contents file to V2 and create a backup of the V1 file.
        /// </summary>
        /// <param name="filename">The V1 table of contents file to migrate.</param>
        /// <returns>The result of the migration.</returns>
        public static TableOfContentsV2 MigrateV1ToV2File(string filename)
        {
            var tocV1 = ReadFromFile<TableOfContentsV1>(filename);
            var tocV2 = ConvertToV2(tocV1);
            tocV2.CoverHash = TableOfContentsV2.ComputeHash(Path.GetDirectoryName(filename));
            File.Copy(filename, filename + "_v1bak");
            JsonWriter.WriteToFilename(filename, tocV2);
            return tocV2;
        }

        /// <summary>
        /// Convert the given V1 table of contents object to V2.
        /// </summary>
        /// <param name="tocV1">The V1 table of contents object.</param>
        /// <returns>The V2 table of contents object based on the values of the V1 object.</returns>
        private static TableOfContentsV2 ConvertToV2(TableOfContentsV1 tocV1)
        {
            var tocV2 = new TableOfContentsV2();
            tocV1.TrackList.ForEach(trackV1 => {
                var trackV2 = new TrackV2 {
                    Artist = tocV1.Artist,
                    Album = tocV1.Album,
                    Genre = tocV1.Genre,
                    Year = tocV1.Year,

                    TrackNumber = trackV1.Number,
                    TrackTitle = trackV1.Title,

                    Filename = trackV1.Filename
                };
                trackV2.UpdateHash();
                tocV2.TrackList.Add(trackV2);
            });
            tocV2.TrackList.Sort((t1, t2) => string.Compare(t1.TrackNumber, t2.TrackNumber, StringComparison.Ordinal));

            return tocV2;
        }
    }
}
