using System.IO;
using MusicManagementCore.Constant;
using MusicManagementCore.Domain.Config;
using MusicManagementCore.Domain.ToC;

namespace MusicManagementCore.Util;

/// <summary>
/// Builds a relative filesystem-conforming file path for an audio track.
///
/// The path is based on a track's meta data and the configured output format. The resulting string
/// is "absolute" relative to the output folder of a given converter configuration.
/// </summary>
/// <param name="format">The configured output format string.</param>
public class TrackFilePathBuilder(Format format)
{
    public string Build(TrackV2 track)
    {
        return Path.Join(BuildPath(track), BuildFile(track));
    }
    
    public string BuildPath(TrackV2 track)
    {
        return DoReplace(format.Path, track);
    }
    
    public string BuildFile(TrackV2 track)
    {
        return DoReplace(format.File, track);
    }

    private static string DoReplace(string format, TrackV2 track)
    {
        return format
            .Replace(MetaTagName.Artist,
                FileSystemUtil.RemoveInvalidFileNameChars(track.IsCompilation
                    ? CompilationArtist.Name
                    : track.Artist))
            .Replace(MetaTagName.Album, FileSystemUtil.RemoveInvalidFileNameChars(track.Album))
            .Replace(MetaTagName.Genre, FileSystemUtil.RemoveInvalidFileNameChars(track.Genre))
            .Replace(MetaTagName.Year, track.Year)
            .Replace(MetaTagName.TrackNumber, track.TrackNumber)
            .Replace(MetaTagName.Title,
                FileSystemUtil.RemoveInvalidFileNameChars(track.TrackTitle));
    }
}