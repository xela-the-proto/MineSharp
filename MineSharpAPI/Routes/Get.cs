using System.Reflection;
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

        app.MapGet("/debug", () =>
        {
            return AppContext.BaseDirectory;
        });
        */
        
        //I bodies servono per poter prendere dal body della richiesta una classe già preparata da poi manipoalre
        app.MapGet("/auth/", async ([FromBody] LoginBody inquilino, HttpContext http, DatabaseContext db, IAuth auth, [FromServices]IDbUser userTable) =>
        {
            
           
            
            var result = auth.Authenticate(db, inquilino, builder, http).Result;
            return result;
        });
    }
}