using Runner.DownloadManager;

namespace Runner;

class Program
{
    static void Main(string[] args)
    {
        var tuple = ArgsParser.ParseArgs(args);
        var listOfFlags = tuple.Item1;
        var listOfValues = tuple.Item2;
        
        DownloadDispatch.DownloadJar(listOfValues[listOfFlags.IndexOf("-v")],listOfValues[listOfFlags.IndexOf("-f")]);
    }
}