using Common.Converters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Bodies;
using Runner.DownloadManager;
using Runner.RunnerManager;

namespace Runner.Routes;

public class Get
{
    public static void RegisterGets(WebApplication app)
    {
        app.MapPost("/startServer", async ([FromBody] RunnerBody tuple) =>
        {
            var args = ArgsParser.BuildArgs(tuple);

            DownloadDispatch.DownloadJar(args[args.IndexOf("-v") + 1], args[args.IndexOf("-f") + 1]);
            var runner = new ServerRunner();

            runner.StartServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1],
                tuple.eulaAccept);
            return Results.Ok();
        });

        

        
    }
}