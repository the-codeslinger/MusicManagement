using MusicManagementCore.Constant;
using MusicManagementCore.Json;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MusicManagementCore.Util
{
    public class JsonWriter
    {
        public static void WriteToDirectory(string directory, TableOfContentsV2 toc)
        {
            WriteToFilename(Path.Combine(directory, StandardFilename.TableOfContents), toc);
        }

        public static void WriteToFilename(string filename, TableOfContentsV2 toc)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(toc, options);
            File.WriteAllText(filename, jsonString);
        }
    }
}
