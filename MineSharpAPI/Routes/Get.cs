using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        app.MapGet("/debug", async (HttpContext http, [FromServices]IDbContextFactory<DatabaseContext> database) =>
        {
            
        });


        app.MapGet("/auth/",
            async ([FromBody] LoginBody user, HttpContext http, [FromServices]IDbContextFactory<DatabaseContext> database, IAuth auth,
                [FromServices] IDbUser userTable) =>
            {
                var result = auth.Authenticate(database.CreateDbContext(), user, builder, http).Result;
                return result;
            });
    }
}