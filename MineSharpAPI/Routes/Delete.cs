using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;
using RestSharp;

namespace MineSharpAPI.Routes;

public class Delete
{
    public static void RegisterDeletes(WebApplication app)
    {
        app.MapDelete("/api/Runners/StopServer", async (HttpContext context,
            [FromServices]IDbContextFactory<DatabaseContext> database) =>
        {
            var serverId = new StreamReader(context.Request.Body).ReadToEndAsync().Result;
            var db = await database.CreateDbContextAsync();
            var server = db.Server.First( x => x.id == serverId).wsPort;
            //TODO:REMOVE ALL LOCALHOST DEPENDENCIES, THIS NEEDS TO WORK OVER THE INTERNET
            using (var client = new RestClient("http://localhost:5001"))
            {
                client.Post(new RestRequest("/stopServer", Method.Post).AddBody(server));
            }
        });
    }
}