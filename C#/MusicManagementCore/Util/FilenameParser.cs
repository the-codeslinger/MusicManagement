using System;
using System.IO;
using System.Linq;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;

namespace MusicManagementCore.Util
{
    /// <summary>
    /// Extract the <cref>MetaData</cref> of an uncompressed, unprocessed audio file's
    /// filename based on a <cref>MetaDataInputConfig</cref>. This configuration
    /// determines the tags to read and the order they appear in the filename.
    /// </summary>
    public class FilenameParser(FilenameEncodingConfig config)
    {
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
            var exploded = name.Split(config.Delimiter);

            var metaData = new MetaData();
            foreach (var zipped in exploded.Zip(config.TagFormat, Tuple.Create)) {
                InsertData(zipped.Item1, zipped.Item2, metaData);
            }

            return metaData;
        }

        private void InsertData(string value, string tag, MetaData metaData)
        {
            switch (tag) {
                case MetaTagName.Artist:
                    metaData.Artist = config.ReplaceCodeStrings(value);
                    break;

                case MetaTagName.Album:
                    metaData.Album = config.ReplaceCodeStrings(value);
                    break;

                case MetaTagName.Genre:
                    metaData.Genre = config.ReplaceCodeStrings(value);
                    break;

                case MetaTagName.Year:
                    metaData.Year = value;
                    break;

                case MetaTagName.TrackNumber:
                    metaData.TrackNumber = value;
                    break;

                case MetaTagName.Title:
                    metaData.TrackTitle = config.ReplaceCodeStrings(value);
                    break;

                default:
                    // TODO Define better exception.
                    throw new Exception("Unrecognized tag " + tag);
            }
        }
    }
}
