using MineSharpAPI.Api;
using MineSharpAPI.Modules.Bodies;

namespace MineSharpAPI.Modules.Interfaces;
/*
 * Interfacce per la DI (vedi Program.cs per cosa è la dependency injection)
 */
public interface IDbUser
{
    UserTable GetUser(DatabaseContext context, UserTable user);
    void RmUser(DatabaseContext context, UserTable user);
    public void SetUser(DatabaseContext context, LoginBody user);
}
