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
        app.MapGet("/auth/", async ([FromBody] LoginBody user, HttpContext http, DatabaseContext db, IAuth auth, [FromServices]IDbUser userTable) =>
        {
            var result = auth.Authenticate(db, user, builder, http).Result;
            return result;
        });

        app.MapGet("/api/runners/GetMasterRunnerToken", async (IAuth auth) =>
        {
            var token = auth.Authenticate(builder.Configuration);
            return token;
        }).RequireAuthorization();

        app.MapGet("/api/runners/GetSingularToken", async () =>
        {

        }).RequireAuthorization();
    }
}