using System.Diagnostics;

using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC.V3;
using MusicManagementCore.Util;

namespace ConvertMusic;

public class FileCompressor(Converter converter)
{
    public void Compress(string tocDir, string source, TrackV3 track)
    {
        var destinationFileName = Path.Combine(converter.Output.Path,
            track.Files.Compressed + "." + converter.Type.ToLower());
        if (File.Exists(destinationFileName)) {
            return;
        }

        MakeDestinationFolder(destinationFileName);
        Compress(source, destinationFileName);
        AudioTagWriter.WriteTags(tocDir, destinationFileName, track);
    }

    /// <summary>
    /// Create the complete folder hierarchy to store the compressed audio file based.
    /// </summary>
    private static void MakeDestinationFolder(string destinationFileName)
    {
        var directory = Path.GetDirectoryName(destinationFileName);
        if (null != directory && !Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Compress the given audio file with the <cref>Converter</cref> provided in the constructor. 
    /// The compressed file will be written to <cref>destinationFileName</cref>.
    /// </summary>
    /// <param name="uncompressedFileName">The uncompressed source audio file.</param>
    /// <param name="destinationFileName">The compressed audio file name.</param>
    private void Compress(string uncompressedFileName, string destinationFileName)
    {
        var args = converter.Command.Args.ConvertAll(arg => {
            return arg switch {
                ConverterArgs.Input => uncompressedFileName,
                ConverterArgs.Output => destinationFileName,
                _ => arg
            };
        });
        var process = Process.Start(converter.Command.Bin, args);
        process.WaitForExit();
    }
}