using CommandLine;

namespace CreateToc
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var convertMusic = new CreateToc(parserResult.Value);
            return convertMusic.Run();
        }
    }
}
