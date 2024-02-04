using CommandLine;

namespace CreateToc
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var createToc = new CreateToc(parserResult.Value);
            return createToc.Run();
        }
    }
}
