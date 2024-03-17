using MusicManagementCore.Constant;
using System.Text.Json.Serialization;

namespace MusicManagementCore.Domain.ToC.V2
{
    public class Files
    {
        [JsonPropertyName(JsonPropertyName.FileOriginal)]
        public string Original { get; set; }

        [JsonPropertyName(JsonPropertyName.FileUncompressed)]
        public string Uncompressed { get; set; }

        [JsonPropertyName(JsonPropertyName.FileCompressed)]
        public string Compressed { get; set; }
    }
}
