using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using MineSharpAPI.Modules.Bodies;
using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Api;
using PusherServer;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/Runners/RunServer", async ([FromBody]RunnerBody body, HttpContext context, DatabaseContext db) =>
        {
            
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;    
            runner.Start();
        });
        
        app.MapPost("/api/Runners/CreateServer", async ([FromBody]RunnerBody body, HttpContext context, DatabaseContext db) =>
        {
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;    
            runner.Start();
        });
        
        
    }
}