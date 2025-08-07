using MineSharpAPI.Modules.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MineSharpAPI.Api;

public class Delete
{
    public static void RegisterDeletes(WebApplication app)
    {
        /*
        app.MapDelete("/api/user/rm", async ([FromBody] InquilinoDB inquilino, HttpContext http,
            DatabaseContext db, IDbInquilino dbInquilino) =>
        {
            dbInquilino.RmInquilino(db, inquilino);
            return Task.CompletedTask;
        }).RequireAuthorization();
        */
    }
}