#define CLI_DEBUG

using System.Diagnostics;
using Common.Json.Structures;
using Runner.DownloadManager;
using Runner.RunnerManager;
using Serilog;

namespace Runner;
class Program
{
    
    //Path to the folder
    public static string BACKEND_TOKEN;
    public static string ABSOLUTE_SERVER_PATH;
    public static string ABSOLUTE_RUNNER_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;
    static void Main(string[] args)
    {
        var tuple = ArgsParser.ParseArgs(args);
        var listOfFlags = tuple.Item1;
        var listOfValues = tuple.Item2;
        
        ConfigChecks(listOfFlags, listOfValues);
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "log.txt")
            .MinimumLevel.Verbose()
            .CreateLogger();
        
        DownloadDispatch.DownloadJar(listOfValues[listOfFlags.IndexOf("-v")],listOfValues[listOfFlags.IndexOf("-f")]);

        var runner = new ServerRunner();
        runner.startServerProcess(listOfFlags,listOfValues,listOfValues[listOfFlags.IndexOf("-f")]);
    }


    public static void ConfigChecks(List<string> listOfFlags, List<string> listOfValues)
    {
        ABSOLUTE_SERVER_PATH = listOfValues[listOfFlags.IndexOf("-f")].Substring(0, 
            listOfValues[listOfFlags.IndexOf("-f")].LastIndexOf(Path.DirectorySeparatorChar));

        if (!Directory.Exists(ABSOLUTE_SERVER_PATH))
        {
            Log.Verbose("DIrectory doesnt exist creating");
            Directory.CreateDirectory(ABSOLUTE_SERVER_PATH);
            if (!File.Exists(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "config.json"))
            {
                Log.Verbose("Config doesnt exist creating");
                RUNNER_PROPERTIES = ConfigManager.WriteConfig(ABSOLUTE_SERVER_PATH);
            }
        }
        
        if (RUNNER_PROPERTIES.token == "")
        {
            Log.Verbose("No auth token!");
            Log.Fatal("Runner succesfully installed! Please enter the token in the config file before running again!");
            var local = Process.GetCurrentProcess();
            
        }
    }
    
}