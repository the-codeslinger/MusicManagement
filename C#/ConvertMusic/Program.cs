using CommandLine;

namespace ConvertMusic
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);
            var convertMusic = new ConvertMusic(parserResult.Value);
            return convertMusic.Run();
        }
    }
}
