using System.IO;

namespace MusicManagementCore.Domain.Audio;

/// <summary>
/// Represents an uncompressed source audio file as found on the filesystem.
/// </summary>
public class UncompressedFile
{
    /// <summary>
    /// Create a new uncompressed audio file from a given file name.
    /// </summary>
    /// <param name="fileName">Absolute or relative file name of the audio file.</param>
    public UncompressedFile(string fileName)
    {
        AbsolutePath = Path.GetFullPath(fileName);
        FileName = Path.GetFileName(AbsolutePath);
        Directory = Path.GetDirectoryName(AbsolutePath);
    }

    /// <summary>
    /// The name of the audio file including the extension and excluding the directory (for example
    /// "01 - Winter's Gate.wav").
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The name of the audio file including the extension and the absolute directory (for example
    /// "D:\Music\Insomnium\01 - Winter's Gate.wav").
    /// </summary>
    public string AbsolutePath { get; }

    /// <summary>
    /// The absolute name of the directory where the audio file was found (for example "D:\Music\Insomnium").
    /// </summary>
    public string Directory { get; }
}
