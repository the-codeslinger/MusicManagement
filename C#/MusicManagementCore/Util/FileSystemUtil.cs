using System.IO;
using System.Linq;

namespace MusicManagementCore.Util;

public static class FileSystemUtil
{
    /// <summary>
    /// Remove all invalid characters from the given string and return a copy.
    /// </summary>
    public static string RemoveInvalidFileNameChars(string value)
    {
        return Path.GetInvalidFileNameChars()
            .Aggregate(value, (current, c) => current.Replace(c.ToString(), ""));
    }
}