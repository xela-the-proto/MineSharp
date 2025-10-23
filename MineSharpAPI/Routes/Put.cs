using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;
using Server = MineSharpAPI.Modules.Bodies.Server;

namespace MineSharpAPI.Routes;

public class Put
{
    public static void RegisterPuts(WebApplication app)
    {
        app.MapPut("/api/user/register", async ([FromBody] LoginBody user, HttpContext http,
            [FromServices]IDbContextFactory<DatabaseContext> database) =>
        {
            var db = database.CreateDbContextAsync().Result;
            db.User.Add(new User
            {
                Email = user.email,
                Id = Guid.NewGuid().ToString(),
                PasswordHash = HashingUtils.HashString(user.password)
            });
            await db.SaveChangesAsync();
        }).RequireAuthorization();


        app.MapPut("/api/runners/register",
            async ([FromBody] Runners runnerDetails) => { });
        
        app.MapPut("/api/runners/updateServerStatus", async ([FromBody] Server serverStats, HttpContext context,
            [FromServices]IDbContextFactory<DatabaseContext> DbFactory) =>
        {
            await using var db = await DbFactory.CreateDbContextAsync();

            var server = new Modules.Api.Server();

            var entity = await db.Server.FirstOrDefaultAsync(x => x.id == serverStats.id);
            if (entity == null)
            {
                server.id = serverStats.id;
                server.name = serverStats.name;
                server.parentRunner = serverStats.parentRunner;
                server.status = serverStats.status;
                server.usage = serverStats.usage;
                server.wsPort = serverStats.wsPort;
                server.IsEulaAccepted = serverStats.IsEulaAccepted;

                await db.Server.AddAsync(server);
            }
            else
            {
                entity.status = serverStats.status;
                entity.usage = serverStats.usage;
                entity.wsPort = serverStats.wsPort;
                db.Server.Update(entity);
            }

            await db.SaveChangesAsync();
        });
        
        app.MapPut("/api/runners/updateEulaStatus", async ([FromBody] EulaUpdateBody body, [FromServices]IDbContextFactory<DatabaseContext> database) =>
        {
            var db = database.CreateDbContext();

            var server = await db.Server.FirstOrDefaultAsync(x => x.id == body.serverId);
            
            if (server == null)
            {
                return Results.NotFound(new { message = "Server not found" });
            }

            server.IsEulaAccepted = body.isEulaAccepted;
            await db.SaveChangesAsync();

            return Results.Ok(new { message = "EULA status updated successfully", server });
        });
    }
}