using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Api;
using MineSharpAPI.Modules.Interfaces;

namespace MineSharpAPI.Modules.Api;

public class Tokens
{
    public static async Task<IResult> CreateApiToken(HttpContext http, DatabaseContext db)
    {

        var auth = http.RequestServices.GetRequiredService<IAuth>();

        try
        {
            byte[] buffer = new byte[2048];
            var token = auth.GenApiKey();
            var user = http.User.Claims.ToList()[1].Value;

            using var reader = new StreamReader(http.Request.Body);
            await reader.BaseStream.ReadAsync(buffer, 0, buffer.Length);

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