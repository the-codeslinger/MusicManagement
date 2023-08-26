using CommandLine;

namespace UpdateTags
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var updateTags = new UpdateTags(parserResult.Value);
            return updateTags.Run();
        }
    }
}