
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
    public static string BACKEND_TOKEN;
    public static string ABSOLUTE_SERVER_PATH;
    public static string ABSOLUTE_RUNNER_PATH;
    public static RunnerPropertiesStructure RUNNER_PROPERTIES;
    static async Task Main()
    {
        var args = ArgsParser.BuildArgs(new RunnerBody()
        {
            path = "/home/alex/serverstest/server2",
            platform = "VANILLA",
            ram = 1024,
            remoteUrl = "http://localhost:5001",
            version = "1.21"
        });
        
        ConfigChecks(args);
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(ABSOLUTE_SERVER_PATH + Path.DirectorySeparatorChar + "log.txt")
            .MinimumLevel.Verbose()
            .CreateLogger();
        
        var javaArgs = ConvertFlagsToJavaFlags.ConvertList(args);

        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();


        app.MapPost("/startServer",([FromBody]RunnerBody tuple) =>
        {
            DownloadDispatch.DownloadJar(args[args.IndexOf("-v") + 1],args[args.IndexOf("-f") + 1]);
            var runner = new ServerRunner();
            
            runner.startServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1]);
            return Results.Ok();
        });
        
        Log.Information("Runner in ascolto su http://localhost:5001/");
        await app.RunAsync("http://localhost:5001");
        
    }


    public static void ConfigChecks(List<string> args)
    {
        ABSOLUTE_SERVER_PATH = args[args.IndexOf("-f") + 1];
        //.Substring(0, args[args.IndexOf("-f") + 1].LastIndexOf(Path.DirectorySeparatorChar));

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
        
        if (RUNNER_PROPERTIES.token == "")
        {
            Log.Verbose("No auth token!");
            Log.Fatal("Runner succesfully installed! Please enter the token in the config file before running again!");
            var local = Process.GetCurrentProcess();
        }
    }
    
}