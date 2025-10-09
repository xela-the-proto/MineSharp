using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Interfaces;

namespace MineSharpAPI.Modules.Api;

public class Auth : IAuth
{
    /*
     * Authing
     */
    public async Task<IResult> Authenticate(DatabaseContext db, LoginBody inquilino,
        WebApplicationBuilder builder, HttpContext httpContext)
    {
        var user = db.User.FirstOrDefault(s => s.Email == inquilino.email);
        if (!httpContext.Request.Cookies.ContainsKey("jwt"))
            return Results.Unauthorized();

        var config = builder.Configuration;
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);

        var claims = new[]
        {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        httpContext.Response.Cookies.Append("jwt", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
            HttpOnly = true,
            Secure = true
        });

        return Results.Ok();
    }

    public string GenApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        var base64String = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_");

        var keyLength = 32 - "MS-".Length;

        return "MS-" + base64String[..keyLength];
    }
}