using CommandLine;

namespace CreateToc
{
    internal class Options
    {
        [Option('c', "config", HelpText = "Path to the configuration file.")]
        public required string Config { get; set; }
    }
}
