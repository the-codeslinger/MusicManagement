using CommandLine;

namespace CreateToc
{
    internal class Options
    {
        [Option('c', "config", HelpText = "Path to the configuration containing <Input> and <FilenameEncoding>.")]
        public required string Config { get; set; }
    }
}
