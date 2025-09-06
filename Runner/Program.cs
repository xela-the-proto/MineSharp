using Common.Json.Structures;
using Runner.DownloadManager;
using Runner.RunnerManager;

namespace Runner;

class Program
{
    public static string ABSOLUTE_SERVER_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;
    static void Main(string[] args)
    {
        if (!File.Exists("config.json"))
        {
            ConfigManager.WriteConfig();
        }
        var tuple = ArgsParser.ParseArgs(args);
        var listOfFlags = tuple.Item1;
        var listOfValues = tuple.Item2;
        
        DownloadDispatch.DownloadJar(listOfValues[listOfFlags.IndexOf("-v")],listOfValues[listOfFlags.IndexOf("-f")]);

        var runner = new ServerRunner();
        runner.startServerProcess(listOfFlags,listOfValues,listOfValues[listOfFlags.IndexOf("-f")]);
    }
}