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
        app.MapGet("/debug", (HttpContext http, DatabaseContext database) =>
        {
            var user = http.User.Claims.ToList();
            return http.User.Claims.ToList()[1].Value;
        }).RequireAuthorization();
        
        
        //I bodies servono per poter prendere dal body della richiesta una classe già preparata da poi manipoalre
        app.MapGet("/auth/", async ([FromBody] LoginBody user, HttpContext http, DatabaseContext db, IAuth auth, [FromServices]IDbUser userTable) =>
        {
            var result = auth.Authenticate(db, user, builder, http).Result;
            return result;
        });

        app.MapGet("/api/runners/GetAPITokenAuth", async (HttpContext http, DatabaseContext db) =>
        {
            var result = Tokens.ValidateApiToken(http, db,builder);
            return result.Result;
        }).RequireAuthorization();
        
    }
}