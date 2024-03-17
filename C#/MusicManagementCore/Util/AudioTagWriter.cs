using System;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Audio;
using MusicManagementCore.Domain.ToC.V2;
using TagLib;

namespace MusicManagementCore.Util;

public class AudioTagWriter
{
    /// <summary>
    /// Write the compressed audio file's meta tags based on the <cref>TrackV2</cref>
    /// information provided in the constructor.
    /// </summary>
    /// <param name="tocDir">The directory of the source file that also contains the cover art
    /// file.</param>
    /// <param name="compressedFileName">The compressed audio absolute file name.</param>
    /// <param name="track">The audio file's meta data.</param>
    public static void WriteTags(string tocDir, string compressedFileName, Track track)
    {
        var file = File.Create(compressedFileName);

        file.Tag.Album = track.MetaData.Album;
        file.Tag.Title = track.MetaData.Title;
        file.Tag.Track = Convert.ToUInt32(track.MetaData.TrackNumber);
        file.Tag.Year = Convert.ToUInt32(track.MetaData.Year);
        file.Tag.Genres = [track.MetaData.Genre];
        file.Tag.Performers = [track.MetaData.Artist];
        file.Tag.AlbumArtists = [track.IsCompilation ? CompilationArtist.Name : track.MetaData.Artist];
        file.Tag.Pictures = [CoverArt.OfDirectory(tocDir).FrontCover];

        file.Save();
    }
}