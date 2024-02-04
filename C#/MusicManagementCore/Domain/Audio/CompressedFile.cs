using System;
using System.Diagnostics;
using System.IO;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;
using MusicManagementCore.Util;

namespace MusicManagementCore.Domain.Audio
{
    /// <summary>
    /// Represents an audio file ready to be compressed. This class is used to facilitate
    /// creating compressed audio files.
    /// </summary>
    /// <param name="musicManagementConfig">The complete configuration. Aspects of all subsection
    /// are required.</param>
    /// <param name="converter">The exact converter to use for compression.</param>
    /// <param name="track">The table of contents track information for this particular audio
    /// file.</param>
    public class CompressedFile(
        MusicManagementConfig musicManagementConfig,
        Converter converter,
        TrackV2 track)
    {
        /// <summary>
        /// Return whether the compressed audio file exists at <cref>DestinationFilename</cref>
        /// or not.
        /// </summary>
        public bool Exists => File.Exists(DestinationFilename);

        /// <summary>
        /// Create the complete folder hierarchy to store the compressed audio file based on
        /// <cref>DestinationFilename</cref>.
        /// </summary>
        public void MakeDestinationFolder()
        {
            var directory = Path.GetDirectoryName(DestinationFilename);
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
        public void WriteAudioTags(string uncompressedSourceDir)
        {
            var file = TagLib.File.Create(DestinationFilename);

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
        public void Compress(string uncompressedFilename)
        {
            var args = converter.Command.Args.ConvertAll(arg =>
            {
                return arg switch
                {
                    ConverterArgs.Input => uncompressedFilename,
                    ConverterArgs.Output => DestinationFilename,
                    _ => arg
                };
            });
            var process = Process.Start(converter.Command.Bin, args);
            process.WaitForExit();
        }

        /// <summary>
        /// Generates and returns the compressed audio file's destination filename.
        /// 
        /// The filename is computed every time. Changes to the track's info result in a 
        /// different filename.
        /// </summary>
        public string DestinationFilename =>
            "new TrackFilePathBuilder(musicManagementConfig.OutputConfig.Format).Build(track)";
    }
}