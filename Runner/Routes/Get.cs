using Common.Json;
using Microsoft.AspNetCore.Mvc;
using Runner.Api;
using WatsonWebsocket;

namespace Runner.Routes;

public class Get
{
    public static void RegisterGets(WebApplication app)
    {
        app.MapPost("/startServer", async ([FromBody] RunnerBody serverDetails) =>
        {
            //Neede so that the api doesnt become a bitch with exceptions because of a bad socket close
            Task.Run(() => new CentralBroker().startServer(serverDetails));
            return Results.Ok();
        });
 
        app.MapPost("/stopServer", async (HttpContext context) =>
        {
            var id = int.Parse(new StreamReader(context.Request.Body).ReadToEndAsync().Result);
            using (var ws = new WatsonWsClient($"localhost", id))
            {
                ws.Start();
                await ws.SendAsync("stop");
            }

        });
    }
}