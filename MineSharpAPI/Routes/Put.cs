using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;

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
    }
}