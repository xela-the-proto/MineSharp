
using System.Diagnostics;
using Common.Converters;
using Common.Enums;
using Common.Json.Structures;
using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Bodies;
using Runner.DownloadManager;
using Runner.RunnerManager;
using Serilog;

namespace Runner;
class Program
{
    
    //Path to the folder
    public static string ABSOLUTE_SERVER_PATH;
    public static string ABSOLUTE_RUNNER_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;
    static async Task Main()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSerilog();
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "log.txt")
            .MinimumLevel.Verbose()
            .CreateLogger();
        Log.Information("Switching to newtonsoft logger");   

        var app = builder.Build();
        
        app.MapPost("/startServer",([FromBody]RunnerBody tuple) =>
        {
            var args = ArgsParser.BuildArgs(tuple);
            ConfigChecks(tuple.path);

            DownloadDispatch.DownloadJar(args[args.IndexOf("-v") + 1],args[args.IndexOf("-f") + 1]);
            var runner = new ServerRunner();
            
            runner.StartServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1]);
            return Results.Ok();
        });
        
        Console.WriteLine("Runner listening http://localhost:5001/");
        await app.RunAsync("http://localhost:5001");
        
    }


    public static void ConfigChecks(string root)
    {
        ABSOLUTE_SERVER_PATH = root;

        if (!Directory.Exists(ABSOLUTE_SERVER_PATH))
        {
            Log.Verbose("Directory doesnt exist creating");
            Directory.CreateDirectory(ABSOLUTE_SERVER_PATH);
            if (!File.Exists(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "config.json"))
            {
                Log.Verbose("Config doesnt exist creating");
                RUNNER_PROPERTIES = ConfigManager.WriteConfig(ABSOLUTE_SERVER_PATH);
            }
        }
    }
    
}