using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace MineSharpAPI.Routes;

public class Get
{
    public static void RegisterGets(WebApplication app, WebApplicationBuilder builder)
    {
        /*
        app.MapGet("api/user/getDetails", async ([FromBody] InquilinoDB inquilino, DatabaseContext db, [FromServices]IDbInquilino dbInquilino) =>
        {
            var result = dbInquilino.GetInquilino(db, inquilino);
            return result;
        }).RequireAuthorization();
        */
        app.MapGet("/debug", async (HttpContext http, DatabaseContext database) => { });
        app.MapGet("/error", async () =>
        {
            JToken? token = null;
            using (var client = new RestClient("https://httpducks.com/"))
            {
                var address = client.GetAsync(new RestRequest("/404.json")).Result;
                var jobjetc = JObject.Parse(address.Content);
                token = jobjetc.SelectToken("image.webp");
                var httpclient = new HttpClient();
            }

            var html = $"<img src=\"{token}\" width=\"500\" height=\"500\">";
            return Results.Content(html, "text/html");
        });


        app.MapGet("/auth/",
            async ([FromBody] LoginBody user, HttpContext http, DatabaseContext db, IAuth auth,
                [FromServices] IDbUser userTable) =>
            {
                var result = auth.Authenticate(db, user, builder, http).Result;
                return result;
            });
    }
}