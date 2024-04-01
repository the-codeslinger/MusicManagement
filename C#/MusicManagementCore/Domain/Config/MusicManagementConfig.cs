using System;
using System.IO;
using System.Text.Json;

namespace MusicManagementCore.Domain.Config;

/// <summary>
/// Class to read the JSON configuration file.
/// </summary>
public class MusicManagementConfig
{
    /// <summary>
    /// Read the configuration from the given file name.
    ///
    /// The JSON file will be read immediately and kept in memory.
    /// </summary>
    /// <param name="configFileName">Absolute or relative file name of the JSON configuration.</param>
    public MusicManagementConfig(string configFileName)
    {
        if (!File.Exists(configFileName)) {
            throw new ArgumentException($"Config file {configFileName} not found.");
        }

        using var stream = new FileStream(configFileName, FileMode.Open, FileAccess.Read);
        using var json = JsonDocument.Parse(stream);

        InputConfig = ToObject<InputConfig>(json, "Input");
        OutputConfig = ToObject<OutputConfig>(json, "Output");
        FileNameEncodingConfig = ToObject<FileNameEncodingConfig>(json, "FilenameEncoding");
    }

    /// <summary>
    /// Get the configuration that contains the source directory scanning info.
    /// </summary>
    public InputConfig InputConfig { get; }

    /// <summary>
    /// Get the configuration that contains how to encode and decode meta data in file names.
    /// </summary>
    public FileNameEncodingConfig FileNameEncodingConfig { get; }

    /// <summary>
    /// Get the configuration that contains all the converters and the output format.
    /// </summary>
    public OutputConfig OutputConfig { get; }

    private static T ToObject<T>(JsonDocument json, string configPropertyName)
    {
        var element = json.RootElement.GetProperty(configPropertyName);
        return element.Deserialize<T>();
    }
}