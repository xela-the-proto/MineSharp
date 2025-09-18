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

    public User GetUser(DatabaseContext context, User inquilino)
    {
        User user = null;
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
        var user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            Email = userLogin.email,
            PasswordHash = hash
        };
        context.User.Add(user);
        context.SaveChanges();
    }
    public void RmUser(DatabaseContext context, User user)
    {
        context.User.Remove(user);
        throw new NotImplementedException();
    }
}