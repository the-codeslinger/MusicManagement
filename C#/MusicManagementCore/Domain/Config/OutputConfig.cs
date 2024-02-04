using System.Collections.Generic;
using MusicManagementCore.Domain.Audio;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace MusicManagementCore.Domain.Config
{
    /// <summary>
    /// Contains the configuration for scanning a source directory for audio files.
    /// 
    /// The following code shows the expected format. It can be a separate JSON object
    /// or part of a bigger JSON object.
    /// 
    /// <code>
    /// {
    ///   "Format": "Genre\\Artist\\Year - Album\\TrackNumber - Title",
    ///   "Converters": [
    ///     {
    ///       "Type": "MP3",
    ///       "Output": {
    ///         "Path": "E:\\Music\\Compressed\\MP3"
    ///       },
    ///       "Command": {
    ///           "Args": [ "-V2", "%input%", "%output%" ],
    ///           "Bin": "C:\\Applications\\Lame\\lame.exe"
    ///       }
    ///     }
    ///   ]
    /// }
    /// </code>
    /// </summary>
    public class OutputConfig
    {
        /// <summary>
        /// Describes the output format that shall be created when writing a compressed
        /// audio file. This can be a folder structure or just a filename. An extension
        /// must not be added. This is determined by the compressor type.
        /// 
        /// It is assumed that the output format shall always be the same for any 
        /// type of converter.
        /// </summary>
        public Format Format { get; set; }

        /// <summary>
        /// A list of all the configured converters to encode audio files from one
        /// format to another.
        /// </summary>
        public List<Converter> Converters { get; set; }
    }
    
    /// <summary>
    /// Defines the path and file format strings.
    /// </summary>
    public class Format
    {
        /// <summary>
        /// The format for the destination directory relative to the converter output path.
        /// </summary>
        public string Path { get; set; }
    
        /// <summary>
        /// The file name format (excluding the path).
        /// </summary>
        public string File { get; set; }
    }

    /// <summary>
    /// Configures a command line application with which the source files shall be
    /// converted to another format.
    /// 
    /// The configuration contains a type, the destination folder, and the command
    /// line utility including the parameters with which it shall be called.
    /// 
    /// The following to replacement value are supported:
    /// <list type="bullet">
    /// <item>
    ///     <term>%input%</term>
    ///     <description>Abolute filename of the file that shall be converted.</description>
    /// </item>
    /// <item>
    ///     <term>%output%</term>
    ///     <description>Abolute filename of the resulting converted file.</description>
    /// </item>
    /// </list>
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// Identifies a specific audio converter and also defines the file extension.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Defines the output configuration, e.g., the destination path where files
        /// shall be written.
        /// </summary>
        public Output Output { get; set; }

        /// <summary>
        /// Describes the converter command line application and how to invoke it.
        /// </summary>
        public Command Command { get; set; }
    }

    /// <summary>
    /// Defines the output configuration, e.g., the destination path where files
    /// shall be written.
    /// </summary>
    public class Output
    {
        /// <summary>
        /// The output directory into which the converted audio files shall be written.
        /// If the output format describes a folder hierarchy, this is the root from
        /// which this hierarchy will be created.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// Describes the converter command line application and how to invoke it.
    /// 
    /// This includes the name of the application, relative or absolute path, and
    /// the command line arguments.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// An ordered list of the command line arguments that will be passed to
        /// the converter executable.
        /// </summary>
        public List<string> Args { get; set; }

        /// <summary>
        /// The name of the application as a relative or an absolute path.
        /// </summary>
        public string Bin { get; set; }
    }
}
