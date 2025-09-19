using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Interfaces;

namespace MineSharpAPI.Api;

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

        app.MapGet("/api/runners/GetSingularToken", async () =>
        {
            
        }).RequireAuthorization();
    }
}