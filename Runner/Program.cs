using System.Reflection;
using Common.Json.Structures;
using Runner.DownloadManager;
using Runner.RunnerManager;
using Serilog;
using Serilog.Events;

namespace Runner;

class Program
{
    //Path to the folder
    public static string BACKEND_TOKEN;
    public static string ABSOLUTE_SERVER_PATH;
    public static string ABSOLUTE_RUNNER_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;
    public static ILogger Log;
    static void Main(string[] args)
    {
        var tuple = ArgsParser.ParseArgs(args);
        var listOfFlags = tuple.Item1;
        var listOfValues = tuple.Item2;
        

        ABSOLUTE_SERVER_PATH = listOfValues[listOfFlags.IndexOf("-f")].Substring(0, 
            listOfValues[listOfFlags.IndexOf("-f")].LastIndexOf(Path.DirectorySeparatorChar));

        var pos = Assembly.GetExecutingAssembly();
        
        
        var log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "log.txt")
            .CreateLogger();
        Log = log;
        
        if (!File.Exists(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "config.json"))
        {
           RUNNER_PROPERTIES = ConfigManager.WriteConfig(ABSOLUTE_SERVER_PATH);
        }

        if (RUNNER_PROPERTIES.token == "")
        {
            Log.Fatal("Runner succesfully installed! Please enter the token in the config file before running again!");
            return;
        }
        
        DownloadDispatch.DownloadJar(listOfValues[listOfFlags.IndexOf("-v")],listOfValues[listOfFlags.IndexOf("-f")]);

        var runner = new ServerRunner();
        runner.startServerProcess(listOfFlags,listOfValues,listOfValues[listOfFlags.IndexOf("-f")]);
    }
}