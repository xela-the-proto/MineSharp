using System.Diagnostics;
using MineSharpAPI.Modules.Bodies;
using Microsoft.AspNetCore.Mvc;
using PusherServer;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/Runners/RunServer", async ([FromBody]RunnerBody body) =>
        {
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;    
            runner.Start();
        });
        
        /*
        app.MapPost("/api/msg/saveToDb", async ([FromBody]MessageBody request, Pusher pusher) =>
        {
            var response = await pusher.TriggerAsync(
                "chat-channel",  // Nome del canale
                "new-message",   // Nome dell'evento
                new { message = request.Text, sender = request.CoinquilinoId }
            );
            
            return Results.Ok(response);
        });
        */
        
    }
}