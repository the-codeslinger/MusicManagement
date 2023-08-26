using System;

namespace MusicManagementCore.Model
{
    /// <summary>
    /// Domain object that contains an audio file's meta data for a single track.
    ///
    /// All meta data is handled as <c>string</c> for simplicity. The data is parsed
    /// from the filename and piped into the compressed file's meta data section.
    /// </summary>
    public class MetaData
    {
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Year { get; set; } = "";
        public string TrackNumber { get; set; } = "";
        public string TrackTitle { get; set; } = "";

        public override bool Equals(object obj)
        {
            return obj is MetaData data
                && Artist == data.Artist
                && Album == data.Album
                && Genre == data.Genre
                && Year == data.Year
                && TrackNumber == data.TrackNumber
                && TrackTitle == data.TrackTitle;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Artist, Album, Genre, Year, TrackNumber, TrackTitle);
        }
    }
}
