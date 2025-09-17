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

    public UserTable GetUser(DatabaseContext context, UserTable inquilino)
    {
        UserTable user = null;
        try
        {
            var file = "x";
            var query =  context.User.First(s => s.Id == inquilino.Id);
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
        var user = new UserTable()
        {
            Id = Guid.NewGuid().ToString(),
            Email = userLogin.email,
            PasswordHash = hash
        };
        context.User.Add(user);
        context.SaveChanges();
    }
    public void RmUser(DatabaseContext context, UserTable user)
    {
        context.User.Remove(user);
        throw new NotImplementedException();
    }
}