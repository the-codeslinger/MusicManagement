using System.Diagnostics;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Domain.ToC.V2;
using MusicManagementCore.Util;

namespace ConvertMusic;

public class FileCompressor(Converter converter)
{
    public void Compress(string tocDir, string source, Track track)
    {
        var destinationFileName = Path.Combine(converter.Output.Path, 
            track.Files.Compressed + "." + converter.Type.ToLower());
        if (File.Exists(destinationFileName)) return;

        MakeDestinationFolder(destinationFileName);
        Compress(source, destinationFileName);
        AudioTagWriter.WriteTags(tocDir, destinationFileName, track);
    }

    /// <summary>
    /// Create the complete folder hierarchy to store the compressed audio file based on
    /// <cref>DestinationFilename</cref>.
    /// </summary>
    private static void MakeDestinationFolder(string destinationFilename)
    {
        var directory = Path.GetDirectoryName(destinationFilename);
        if (null != directory && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Compress the given audio file with the <cref>Converter</cref> provided in the 
    /// constructor. The compressed file will be written to <cref>DestinationFilename</cref>.
    /// </summary>
    /// <param name="uncompressedFilename">The uncompressed source audio file.</param>
    /// <param name="destinationFilename">The compressed audio filename.</param>
    private void Compress(string uncompressedFilename, string destinationFilename)
    {
        var args = converter.Command.Args.ConvertAll(arg =>
        {
            return arg switch
            {
                ConverterArgs.Input => uncompressedFilename,
                ConverterArgs.Output => destinationFilename,
                _ => arg
            };
        });
        var process = Process.Start(converter.Command.Bin, args);
        process.WaitForExit();
    }
}