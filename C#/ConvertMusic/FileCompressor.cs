using System.Diagnostics;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;

namespace ConvertMusic;

public class FileCompressor(Converter converter)
{
    public void Compress(string directory, string relativeOutDir, string source,
        TrackV2 track)
    {
        var destinationFileName = Path.Combine(converter.Output.Path, relativeOutDir,
            track.Filename.OutName + "." + converter.Type.ToLower());
        if (File.Exists(destinationFileName)) return;

        MakeDestinationFolder(destinationFileName);
        Compress(source, destinationFileName);
        WriteAudioTags(directory, destinationFileName, track);
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
    /// Write the compressed audio file's meta tags based on the <cref>TrackV2</cref>
    /// information provided in the constructor.
    /// </summary>
    /// <param name="uncompressedSourceDir">The directory of the source file that also 
    /// contains the cover art file.</param>
    /// <param name="destinationFilename">The compressed audio filename.</param>
    /// <param name="track">The audio file's meta data.</param>
    private static void WriteAudioTags(string uncompressedSourceDir, string destinationFilename,
        TrackV2 track)
    {
        var file = TagLib.File.Create(destinationFilename);

        file.Tag.Album = track.Album;
        file.Tag.Title = track.TrackTitle;
        file.Tag.Track = Convert.ToUInt32(track.TrackNumber);
        file.Tag.Year = Convert.ToUInt32(track.Year);
        file.Tag.Genres = [track.Genre];
        file.Tag.Performers = [track.Artist];
        file.Tag.AlbumArtists = [track.IsCompilation ? CompilationArtist.Name : track.Artist];
        file.Tag.Pictures = [new CoverArt(uncompressedSourceDir).FrontCover];

        file.Save();
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