using System.Text;
using MineSharpAPI.Modules.Interfaces;
using Serilog;

namespace MineSharpAPI.Api;

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

            await db.ApiKeys.AddAsync(new APIKeys()
            {
                Key = token,
                keyName = Encoding.ASCII.GetString(buffer),
                OwnerID = user
            });
            await db.SaveChangesAsync();

            return Results.Ok(token);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Db update error, refer to stacktrace");
            return Results.InternalServerError();

        }
    }
}