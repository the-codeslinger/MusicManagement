using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

using MusicManagementCore.Constant;
using MusicManagementCore.Domain.ToC.V3;

namespace MusicManagementCore.Util;

/// <summary>
/// Helper class for writing table of contents JSON files using <cref>System.Text.Json</cref>.
/// </summary>
public static class JsonWriter
{
    /// <summary>
    /// Write the given table of contents to the given directory using the standard file name
    /// defined in <cref>StandardFileName.TableOfContents</cref>.
    /// </summary>
    /// <param name="directory">The destination directory to write the table of contents file to.</param>
    /// <param name="toc">The table of contents to write.</param>
    public static void WriteToDirectory(string directory, TableOfContentsV3 toc)
    {
        WriteToFileName(Path.Combine(directory, StandardFileName.TableOfContents), toc);
    }

    /// <summary>
    /// Write the given table of contents to the given file.
    /// </summary>
    /// <param name="fileName">The file name of the table of contents file to write.</param>
    /// <param name="toc">The table of contents to write.</param>
    public static void WriteToFileName(string fileName, TableOfContentsV3 toc)
    {
        var options = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(toc, options);
        File.WriteAllText(fileName, jsonString);
    }
}
