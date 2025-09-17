using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MineSharpAPI.Api;

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

        app.MapPut("/api/runners/register", async([FromBody]RunnerTable runnerDetails, HttpContext context, DatabaseContext db) =>
        {
            
        });
    }
}