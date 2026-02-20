using Common.Enums;
using Common.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;
using RestSharp;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/server/RunServer",
            async ([FromBody] RunnerBody body) =>
            {
                using (var client = new RestClient(body.remoteUrl))
                {
                    if (string.IsNullOrEmpty(body.platform))
                    {
                        body.platform = ServerPlatform.VANILLA.ToString();
                    }

                    client.Post(new RestRequest("/startServer", Method.Post).AddBody(body));
                }
            });

        app.MapPost("/api/server/CreateServer",
            async ([FromBody] RunnerBody body) =>
            {
                using (var client = new RestClient(body.remoteUrl))
                {
                    if (string.IsNullOrEmpty(body.platform))
                    {
                        body.platform = ServerPlatform.VANILLA.ToString();
                    }

                    client.Post(new RestRequest("/createserver", Method.Post).AddBody(body));
                }
            });

        app.MapPost("/api/auth/GenAPIToken",
            async (HttpContext http, [FromServices] IDbContextFactory<DatabaseContext> database) =>
            {
                var result = Tokens.CreateApiToken(http, database.CreateDbContext());
                return result.Result;
            }).RequireAuthorization();
    }
}