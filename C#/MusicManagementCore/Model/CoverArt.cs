using MusicManagementCore.Constant;
using System.IO;

namespace MusicManagementCore.Model
{
    /// <summary>
    /// Domain object representing the Cover.jpg file of an audio disc.
    /// </summary>
    public class CoverArt
    {
        private readonly string _directory;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="directory">The directory where the cover art file is located.</param>
        public CoverArt(string directory)
        {
            _directory = directory;
        }

        /// <summary>
        /// The absolute filename of the cover art file.
        /// </summary>
        public string Filename
        {
            get {
                return Path.Combine(_directory, StandardFilename.CoverArt);
            }
        }

        /// <summary>
        /// A Taglib-specific <cref>Taglib.Picture</cref> representing the front cover.
        /// </summary>
        public TagLib.Picture FrontCover
        {
            get {
                return new TagLib.Picture(Filename) {
                    Type = TagLib.PictureType.FrontCover,
                    MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
                };
            }
        }
    }
}
