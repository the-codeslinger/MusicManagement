using System.IO;

namespace MusicManagementCore.Domain.Audio;

/// <summary>
/// Represents a single audio file as found on the filesystem.
/// </summary>
public class UncompressedFile
{
    /// <summary>
    /// The name of the audio file including the extension, excluding the directory
    /// (for example "01 - Winter's Gate.wav").
    /// </summary>
    public string Filename { get; }

    /// <summary>
    /// The name of the audio file including the extension and the absolute
    /// directory (for example "D:\Music\Insomnium\01 - Winter's Gate.wav").
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// The absolute name of the directory where the audio file was found (for
    /// example "D:\Music\Insomnium").
    /// </summary>
    public string Directory { get; }

    /// <summary>
    /// Create a new uncompressed audio file from a given filename and the associated.
    /// </summary>
    /// <param name="filename">Absolute or relative filename of the audio file.</param>
    public UncompressedFile(string filename)
    {
        AbsolutePath = Path.GetFullPath(filename);
        Filename = Path.GetFileName(AbsolutePath);
        Directory = Path.GetDirectoryName(AbsolutePath);
    }
}
