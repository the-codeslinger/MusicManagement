using MusicManagementCore.Config;
using MusicManagementCore.Model;
using System;
using System.IO;
using System.Linq;

namespace MusicManagementCore.Util
{
    /// <summary>
    /// Extract the <cref>MetaData</cref> of an uncompressed, unprocessed audio file's
    /// filename based on a <cref>MetaDataInputConfig</cref>. This configuration
    /// determines the tags to read and the order they appear in the filename.
    /// </summary>
    public class FilenameParser
    {
        private readonly FilenameEncodingConfig _filenameEncodingConfig;

        public FilenameParser(FilenameEncodingConfig config)
            => _filenameEncodingConfig = config;

        /// <summary>
        /// Parse an audio file's filename and extract the meta data according to
        /// the encoding configuration.
        /// </summary>
        /// <param name="filename">The audio file's filename, relative or absolute.</param>
        /// <returns>The meta data that was encoded in the filename. Data that was
        /// not listed in the input configuration will be treated as an empty string.</returns>
        public MetaData ParseMetaData(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var exploded = name.Split(_filenameEncodingConfig.Delimiter);

            var metaData = new MetaData();
            foreach (var zipped in exploded.Zip(_filenameEncodingConfig.TagFormat, Tuple.Create)) {
                InsertData(zipped.Item1, zipped.Item2, metaData);
            }

            return metaData;
        }

        private static void InsertData(string value, string tag, MetaData metaData)
        {
            switch (tag) {
                case Constant.MetaTagName.Artist:
                    metaData.Artist = value;
                    break;

                case Constant.MetaTagName.Album:
                    metaData.Album = value;
                    break;

                case Constant.MetaTagName.Genre:
                    metaData.Genre = value;
                    break;

                case Constant.MetaTagName.Year:
                    metaData.Year = value;
                    break;

                case Constant.MetaTagName.TrackNumber:
                    metaData.TrackNumber = value;
                    break;

                case Constant.MetaTagName.Title:
                    metaData.TrackTitle = value;
                    break;

                default:
                    // TODO Define better exception.
                    throw new System.Exception("Unrecognized tag " + tag);
            }
        }
    }
}
