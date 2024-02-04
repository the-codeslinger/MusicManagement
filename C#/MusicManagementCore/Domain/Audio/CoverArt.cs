using System.IO;
using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.Audio
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
        /// A Taglib-specific <cref>Taglib.Picture</cref> representing the front cover.
        /// </summary>
        public TagLib.Picture FrontCover =>
            new(Filename) {
                Type = TagLib.PictureType.FrontCover,
                MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
            };

        /// <summary>
        /// The absolute filename of the cover art file.
        /// </summary>
        private string Filename => Path.Combine(_directory, StandardFilename.CoverArt);
    }
}
