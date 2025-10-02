using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Interfaces;

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
        app.MapGet("/debug", async (HttpContext http, DatabaseContext database) =>
        {
            var result = Tokens.CreateApiToken(http, database);
            return result.Result;
        });
        
        
        app.MapGet("/auth/", async ([FromBody] LoginBody user, HttpContext http, DatabaseContext db, IAuth auth, [FromServices]IDbUser userTable) =>
        {
            var result = auth.Authenticate(db, user, builder, http).Result;
            return result;
        });
        
    }
}