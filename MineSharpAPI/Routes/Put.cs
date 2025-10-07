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
            DatabaseContext db) =>
        {
            db.User.Add(new User()
            {
                Email = user.email,
                Id = Guid.NewGuid().ToString(),
                PasswordHash = HashingUtils.HashString(user.password)
            });
            await db.SaveChangesAsync();
        }).RequireAuthorization();
        
        

        app.MapPut("/api/runners/register", async([FromBody]Runners runnerDetails, HttpContext context, DatabaseContext db) =>
        {
            
        });
        app.MapPut("/api/runners/updateServerStatus", async ([FromBody] Server serverStats, HttpContext context, IDbContextFactory<DatabaseContext> DbFactory) =>
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
    }
}