using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using MineSharpAPI.Modules.Bodies;
using Microsoft.AspNetCore.Mvc;
using PusherServer;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/Runners/RunServer", async ([FromBody]RunnerBody body, HttpContext context) =>
        {
            
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;    
            runner.Start();
            
            
            
        });
        
        app.MapPost("/api/Runners/CreateServer", async ([FromBody]RunnerBody body, HttpContext context) =>
        {
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;    
            runner.Start();
        });
        
        /*
        app.MapPost("/send", async (HttpContext context) =>
        {
            using var client = new ClientWebSocket();
            var uri = new Uri("ws://127.0.0.1:778");

            // Connetti al websocket
            await client.ConnectAsync(uri, CancellationToken.None);

            // Leggi testo dall’HTTP request
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8);
            var message = await reader.ReadToEndAsync();

            // Manda il messaggio
            var buffer = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            // Ricevi la risposta
            var responseBuffer = new byte[4096];
            var result = await client.ReceiveAsync(new ArraySegment<byte>(responseBuffer), CancellationToken.None);

            var responseText = Encoding.UTF8.GetString(responseBuffer, 0, result.Count);

            return Results.Ok(responseText);
        });
        */

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