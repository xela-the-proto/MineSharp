using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Interfaces;
using Serilog;

namespace MineSharpAPI.Modules.Api;

public class Tokens
{
    public static async Task<IResult> CreateApiToken(HttpContext http, DatabaseContext db)
    {
        Log.Debug("Import IAuth");
        var auth = http.RequestServices.GetRequiredService<IAuth>();
        string user = "";
        try
        {
            string body = "";
            var token = auth.GenApiKey();
            user = http.User.Claims.ToList()[1].Value;
            Log.Debug("Start buffer read");

            using (var reader = new StreamReader(http.Request.Body,Encoding.UTF8))
            {
                body = reader.ReadToEndAsync().Result;
            }
            
            Log.Debug("Buffer disposed");
            var apiKey = new APIKeys()
            {
                Key = token,
                keyName = body,
                OwnerID = user
            };
            if (!db.ApiKeys.ContainsAsync(apiKey).Result)
            {
                await db.ApiKeys.AddAsync(apiKey);
                await db.SaveChangesAsync();
            }
            else
            {
                throw new DataException("API key already exists with that name");
            }
            return Results.Ok(token);
        }
        catch (DataException e)
        {
            http.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return Results.InternalServerError($"Api key already exists with that name for user id {user}");
        }
    }
    public static async Task<IResult> ValidateApiToken(HttpContext http, DatabaseContext db, WebApplicationBuilder builder)
    {
        Log.Debug("Import IAuth");
        var auth = http.RequestServices.GetRequiredService<IAuth>();
        string user = "";
        try
        {
            string body = "";
            var token = auth.GenApiKey();
            user = http.User.Claims.ToList()[1].Value;
            Log.Debug("Start buffer read");

            using (var reader = new StreamReader(http.Request.Body,Encoding.UTF8))
            {
                body = reader.ReadToEndAsync().Result;
            }
            
            Log.Debug("Buffer disposed");
            var apiKey = new APIKeys()
            {
                Key = token,
                keyName = body,
                OwnerID = user
            };
            var result = db.ApiKeys.First(x => x.OwnerID == user && x.Key == body);
            /*
            foreach (var key in result)
            {
                Log.Debug(key.Key + " " + key.OwnerID);
            }*/
            return Results.Ok();
        }
        catch (DataException e)
        {
            return Results.InternalServerError();
        }
    }
}