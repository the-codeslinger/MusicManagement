using CommandLine;

namespace ConvertMusic
{
    internal class Options
    {
        [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
        public required string Config { get; set; }

        [Option('f', "format", HelpText = "The converter to use. This identifies the converter in the 'Output' section of the configuration file.")]
        public required string Format { get; set; }
    }
}
