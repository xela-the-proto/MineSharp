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
            byte[] buffer = new byte[2048];
            var token = auth.GenApiKey();
            var user = http.User.Claims.ToList()[1].Value;
            Log.Debug("Start buffer read");
            using (var reader = new StreamReader(http.Request.Body))
            {
                var readBytes =  reader.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                if (readBytes.Result != reader.BaseStream.Length)
                {
                    throw new EndOfStreamException("Read too many bytes than expected");
                }
            }
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
            return Results.InternalServerError(e.Message);
        }
    }
}