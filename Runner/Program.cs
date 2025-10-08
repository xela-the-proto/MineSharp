using Common.Json.Structures;
using Runner.Routes;
using Runner.RunnerManager;
using Serilog;

namespace Runner;

internal class Program
{
    //Path to the folder
    public static string ABSOLUTE_SERVER_PATH;
    public static string CONFIG_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;

    private static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSerilog();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "log.txt")
            .MinimumLevel.Verbose()
            .CreateLogger();
        Log.Information("Switching to Serilog...");

        ConfigChecks(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MinesharpRunner"));
        var app = builder.Build();

        Get.registerGets(app);


        Log.Warning("Runner listening http://localhost:5001/");
        await app.RunAsync("http://localhost:5001");
    }


    public static void ConfigChecks(string root)
    {
        CONFIG_PATH = Path.Combine(root);
        Directory.CreateDirectory(CONFIG_PATH);

        if (!File.Exists(Path.Combine(CONFIG_PATH, "config.json")))
        {
            Log.Verbose("Config doesnt exist creating");
            RUNNER_PROPERTIES = ConfigManager.WriteConfig(CONFIG_PATH);
        }

        RUNNER_PROPERTIES = ConfigManager.ReadConfig(CONFIG_PATH);
    }
}