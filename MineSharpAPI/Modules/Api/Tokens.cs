using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Interfaces;
using Serilog;

namespace MineSharpAPI.Modules.Api;

public class Tokens
{
    public static async Task<IResult> CreateApiToken(HttpContext http, DatabaseContext db)
    {
        Log.Debug("Import IAuth");
        var auth = http.RequestServices.GetRequiredService<IAuth>();

        try
        {
            
            var token = auth.GenApiKey();
            var user = http.User.Claims.ToList()[1].Value;
            Log.Debug("Start buffer read");
            var reader = new StreamReader(http.Request.Body);
            
            byte[] buffer = new byte[reader.BaseStream.Length]; 
            await reader.BaseStream.ReadExactlyAsync(buffer);
            
            reader.Dispose();
            Log.Debug("Buffer disposed");
            var apiKey = new APIKeys()
            {
                Key = token,
                keyName = Encoding.ASCII.GetString(buffer),
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
            return Results.Ok();
        }
        catch (Exception e)
        {
            throw;
            return Results.InternalServerError();
            
        }
    }
}