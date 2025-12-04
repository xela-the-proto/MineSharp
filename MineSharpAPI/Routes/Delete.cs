using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace MineSharpAPI.Routes;

public class Delete
{
    public static void RegisterDeletes(WebApplication app)
    {
        app.MapDelete("/api/Runners/StopServer", async ([FromBody] int processNumber) =>
        {
            using (var client = new RestClient(body.remoteUrl))
            {
                if (string.IsNullOrEmpty(body.platform)) body.platform = ServerPlatform.VANILLA.ToString();
                client.Post(new RestRequest("/stopServer", Method.Post).AddBody(body));
            }
        });
    }
}