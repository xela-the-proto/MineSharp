using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;

namespace MineSharpAPI.Modules.Interfaces;
/*
 * Interfaccia per la DI (vedi Program.cs per cosa è la dependency injection)
 */
public interface IAuth
{
    Task<IResult> Authenticate(DatabaseContext db, LoginBody inquilino, WebApplicationBuilder builder,
        HttpContext httpContext);

    Task<IResult> AuthenticateViaAPIKey(DatabaseContext db, string apiKey, WebApplicationBuilder builder,
        HttpContext httpContext);
    string GenApiKey();
}