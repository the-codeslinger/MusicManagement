using MusicManagementCore.Constant;
using MusicManagementCore.Json;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MusicManagementCore.Util
{
    /// <summary>
    /// Helper class for writing table of contents JSON files using <cref>System.Text.Json</cref>.
    /// </summary>
    public class JsonWriter
    {
        /// <summary>
        /// Write the given table of contents to the given directory using the standard filename
        /// defined in <cref>StandardFilename.TableOfContents</cref>.
        /// </summary>
        /// <param name="directory">The destination directory to write the table of contents 
        /// file to.</param>
        /// <param name="toc">The table of contents to write.</param>
        public static void WriteToDirectory(string directory, TableOfContentsV2 toc)
        {
            WriteToFilename(Path.Combine(directory, StandardFilename.TableOfContents), toc);
        }

        /// <summary>
        /// Write the given table of contents to the given file.
        /// </summary>
        /// <param name="filename">The filename of the table of contents file to write.</param>
        /// <param name="toc">The table of contents to write.</param>
        public static void WriteToFilename(string filename, TableOfContentsV2 toc)
        {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(toc, options);
            File.WriteAllText(filename, jsonString);
        }
    }
}
