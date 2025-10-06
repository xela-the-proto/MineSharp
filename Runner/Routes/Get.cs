using Common.Converters;
using Common.Process;
using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Bodies;
using Runner.Api;
using Runner.DownloadManager;
using Runner.RunnerManager;

namespace Runner.Routes;

public class Get
{
    public static void registerGets(WebApplication app)
    {
        app.MapPost("/startServer",async ([FromBody]RunnerBody tuple) =>
        {
            var args = ArgsParser.BuildArgs(tuple);
            Program.ConfigChecks(tuple.path);

            DownloadDispatch.DownloadJar(args[args.IndexOf("-v") + 1],args[args.IndexOf("-f") + 1]);
            var runner = new ServerRunner();
            
            runner.StartServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1],tuple.eulaAccept);
            return Results.Ok();
        });

        app.MapGet("/debug", async ([FromBody]RunnerBody tuple) =>
        {
            
        });
    }
}