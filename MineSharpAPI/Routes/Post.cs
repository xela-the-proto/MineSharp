using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using RestSharp;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/Runners/RunServer",
            async ([FromBody] RunnerBody body, HttpContext context, DatabaseContext db) =>
            {
                using (var client = new RestClient(body.remoteUrl))
                {
                    if (string.IsNullOrEmpty(body.platform)) body.platform = ServerPlatform.VANILLA.ToString();
                    client.PostAsync(new RestRequest("/startServer", Method.Post).AddBody(body));
                }
            });

        app.MapPost("/api/Runners/CreateServer",
            async ([FromBody] RunnerBody body, HttpContext context, DatabaseContext db) => { });

        app.MapPost("/api/runners/GenAPIToken", async (HttpContext http, DatabaseContext db) =>
        {
            var result = Tokens.CreateApiToken(http, db);
            return result.Result;
        }).RequireAuthorization();
    }
}