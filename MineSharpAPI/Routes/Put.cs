using Microsoft.AspNetCore.Mvc;
using MineSharpAPI.Modules.Api;

namespace MineSharpAPI.Routes;

public class Put
{
    public static void RegisterPuts(WebApplication app)
    {
        /*
        app.MapPut("/api/user/register", async ([FromBody] RegistrationBody inquilino, HttpContext http,
            DatabaseContext db, IDbInquilino dbInquilino) =>
        {
            dbInquilino.SetInquilino(db, inquilino);
            return Task.CompletedTask;
        });//.RequireAuthorization().RequireRateLimiting("Restrictive");

        app.MapPut("/api/billings/add", async ([FromBody]UploadInvoiceBody body, DatabaseContext db,
            IDbBilling dbBilling) =>
        {
            dbBilling.AddFattura(db, body);
        }).RequireAuthorization();
        */

        app.MapPut("/api/runners/register", async([FromBody]Runners runnerDetails, HttpContext context, DatabaseContext db) =>
        {
            
        });
    }
}