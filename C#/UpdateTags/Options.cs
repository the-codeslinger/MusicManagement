using CommandLine;

namespace UpdateTags
{
    internal class Options
    {
        [Option('c', "config",
            HelpText = "Path to the configuration containing <Input> and <Output>.")]
        public required string Config { get; set; }

        [Option('f', "format",
            HelpText =
                "The converter whose output path to use. This locates the files for updating.")]
        public required string Format { get; set; }

        [Option('u', "uncompressed",
            HelpText =
                "The source folder of uncompressed files. Must contain a table of contents file.")]
        public required string Uncompressed { get; set; }

        [Option('o', "compressed",
            HelpText =
                "The folder of compressed files matching the format.")]
        public required string Compressed { get; set; }
    }
}