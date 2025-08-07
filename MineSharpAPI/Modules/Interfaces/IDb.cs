using MineSharpAPI.Api;
using MineSharpAPI.Modules.Bodies;

namespace MineSharpAPI.Modules.Interfaces;
/*
 * Interfacce per la DI (vedi Program.cs per cosa è la dependency injection)
 */
public interface IDbUser
{
    UserDB GetUser(DatabaseContext context, UserDB user);
    void RmUser(DatabaseContext context, UserDB user);
    public void SetUser(DatabaseContext context, LoginBody user);
}
