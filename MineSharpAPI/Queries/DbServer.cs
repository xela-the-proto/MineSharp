using Microsoft.Extensions.Caching.Distributed;
using MineSharpAPI.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Helpers;
using MineSharpAPI.Modules.Interfaces;
using Serilog;

namespace MineSharpAPI.Queries;

public class DbServer : IDbUser
{

    public UserDB GetUser(DatabaseContext context, UserDB inquilino)
    {
        UserDB user = null;
        try
        {
            var file = "x";
            var query =  context.User.First(s => s.UserId == inquilino.UserId);
            return query;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void SetUser(DatabaseContext context, LoginBody userLogin)
    {
        var hash = Hashing.HashString(userLogin.password);
        var user = new UserDB()
        {
            UserId = Guid.NewGuid().ToString(),
            Email = userLogin.email,
            PasswordHash = hash
        };
        context.User.Add(user);
        context.SaveChanges();
    }
    public void RmUser(DatabaseContext context, UserDB user)
    {
        throw new NotImplementedException();
    }
}