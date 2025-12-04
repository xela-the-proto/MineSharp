using System.Diagnostics;
using Common.Converters;
using Common.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            runner.StartServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1]);
            return Results.Ok();
        });

        app.MapPost("/stopServer", async (HttpContext context) =>
        {
            var id = int.Parse(new StreamReader(context.Request.Body).ReadToEndAsync().Result);
            Process process = Process.GetProcessById(id);
            var writer = process.StandardInput;
            writer.WriteLine("stop");
            while (!process.HasExited)
            {
                
            }
        });
    }
}