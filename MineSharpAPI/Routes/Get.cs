using Common.Enums;
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
            async ([FromBody] LoginBody user, HttpContext http, [FromServices]IDbContextFactory<DatabaseContext> database, IAuth auth) =>
            {
                var result = auth.Authenticate(database.CreateDbContext(), user, builder, http).Result;
                return result; 
            });

        app.MapGet("/api/runners/getRunningServers", async ( [FromServices]IDbContextFactory<DatabaseContext> database) =>
        {
            var db = database.CreateDbContext();

            var servers = db.Server.Where(x => x.status == ServerStatus.RUNNING);
            return servers.ToList();
        });
        
        app.MapGet("/api/runners/getEulaStatus", async ([FromBody]string id, [FromServices]IDbContextFactory<DatabaseContext> database, HttpContext context) =>
        {
            var db = database.CreateDbContext();

            var server = await db.Server.FirstOrDefaultAsync(x => x.id == id );

            if (server == null)
            {
                return Results.NotFound("Database doesnt contain record for this server");
            }

            return Results.Ok(server.IsEulaAccepted);
        });
    }
}