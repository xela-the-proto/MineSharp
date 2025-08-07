using MineSharpAPI.Modules.Bodies;
using Microsoft.AspNetCore.Mvc;
using PusherServer;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
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