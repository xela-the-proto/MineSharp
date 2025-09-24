using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Bodies;
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
        if (user == null || !Hashing.HashingUtils.VerifyHash(inquilino.password, user.PasswordHash))
        {
            return Results.Unauthorized();
        }

        var config = builder.Configuration;
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
        /*
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
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        httpContext.Response.Cookies.Append("jwt-user", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
            HttpOnly = true,
            Secure = true
        });
        */
        var token = JwtBuilder.Create().WithAlgorithm(new RS256Algorithm(RSA.Create(),RSA.Create()))
            .AddClaim("exp", DateTimeOffset.Now.AddHours(1).ToUnixTimeSeconds())
            .AddClaim("sub", user.Id)
            .Encode();
        
        httpContext.Response.Cookies.Append("jwt-user", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
            HttpOnly = true,
            Secure = true
        });
        return Results.Ok();
    }
    
    public async Task<IResult> AuthenticateViaAPIKey(DatabaseContext db, string apiKey,
        WebApplicationBuilder builder, HttpContext httpContext)
    {
        var config = builder.Configuration;
        var key = Encoding.ASCII.GetBytes(config["Jwt:Key"]);
        
        var claims = new[]
        {
            new Claim("Id", Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, apiKey),
            new Claim(JwtRegisteredClaimNames.Email, apiKey),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

        httpContext.Response.Cookies.Append("jwt-runner", token, new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(1),
            HttpOnly = true,   
            Secure = true
        });

        return Results.Ok();
    }

    public string GenApiKey()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(32);

        string base64String = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_");
    
        var keyLength = 32 - "MS-".Length; 

        return "MS-" + base64String[..keyLength];
    }
}