using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.Audio;

/// <summary>
/// Domain object representing the Cover.jpg file of an audio disc.
/// </summary>
public class CoverArt
{
    /// <summary>
    /// Construct a CoverArt object from the base directory of the 
    /// <cref>StandardFileName.CoverArt</cref> file.
    /// </summary>
    /// <param name="directory">Directory path that contains the cover art file.</param>
    /// <returns>An instance with the absolute file name of the cover art file.</returns>
    public static CoverArt OfDirectory(string directory)
    {
        var path = System.IO.Path.Combine(directory, StandardFileName.CoverArt);
        return new CoverArt { Path = System.IO.Path.GetFullPath(path) };
    }

    /// <summary>
    /// A Taglib-specific <cref>Taglib.Picture</cref> representing the front cover.
    /// </summary>
    public TagLib.Picture FrontCover =>
        new(Path) {
            Type = TagLib.PictureType.FrontCover,
            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg
        };

    /// <summary>
    /// The absolute file name of the cover art file.
    /// </summary>
    public string Path { get; init; }
}
