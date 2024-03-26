using System;
using System.IO;
using System.Text.Json;

namespace MusicManagementCore.Domain.Config;

/// <summary>
/// Convenience class to read individual parts of JSON configuration files into
/// config objects.
/// </summary>
public class MusicManagementConfig
{
    /// <summary>
    /// Create a new config reader for the given filename.
    ///
    /// The JSON file will be read immediately and kept in memory.
    /// </summary>
    /// <param name="configFilename">Absolute or relative filename of the JSON configuration.</param>
    public MusicManagementConfig(string configFilename)
    {
        if (!File.Exists(configFilename)) {
            throw new ArgumentException($"Config file {configFilename} not found.");
        }

        using var stream = new FileStream(configFilename, FileMode.Open, FileAccess.Read);
        using var json = JsonDocument.Parse(stream);

        InputConfig = ToObject<InputConfig>(json, "Input");
        OutputConfig = ToObject<OutputConfig>(json, "Output");
        FilenameEncodingConfig = ToObject<FilenameEncodingConfig>(json, "FilenameEncoding");
    }

    /// <summary>
    /// Read the part of the configuration file that contains the source directory
    /// scanning information
    /// </summary>
    /// <returns>Returns an <cref>InputConfig</cref> object that contains the 
    /// configuration.</returns>
    public InputConfig InputConfig { get; }

    /// <summary>
    /// Read the part of the configuration file that contains information how to
    /// encode and decode meta data in filenames.
    /// </summary>
    /// <returns>Returns an <cref>FilenameEncodingConfig</cref> object that contains 
    /// the configuration.</returns>
    public FilenameEncodingConfig FilenameEncodingConfig { get; }

    /// <summary>
    /// Read the part of the configuration file that contains all the converters.
    /// </summary>
    /// <returns>Returns an <cref>OutputConfig</cref> object that contains the 
    /// configuration.</returns>
    public OutputConfig OutputConfig { get; }

    private static T ToObject<T>(JsonDocument json, string configPropertyName)
    {
        var element = json.RootElement.GetProperty(configPropertyName);
        return element.Deserialize<T>();
    }
}