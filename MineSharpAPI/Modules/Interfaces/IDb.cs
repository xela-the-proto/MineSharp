using Common.Json;
using MineSharpAPI.Modules.Api;

namespace MineSharpAPI.Modules.Interfaces;

/*
 * Interfacce per la DI (vedi Program.cs per cosa è la dependency injection)
 */
public interface IDbUser
{
    User GetUser(DatabaseContext context, User user);
    void RmUser(DatabaseContext context, User user);
    public void SetUser(DatabaseContext context, LoginBody user);
}