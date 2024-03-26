using System.Text.Json.Serialization;

using MusicManagementCore.Constant;

namespace MusicManagementCore.Domain.ToC.V3;

public class FilesV3
{
    [JsonPropertyName(JsonPropertyName.FileOriginal)]
    public string Original { get; set; }

    [JsonPropertyName(JsonPropertyName.FileUncompressed)]
    public string Uncompressed { get; set; }

    [JsonPropertyName(JsonPropertyName.FileCompressed)]
    public string Compressed { get; set; }
}
