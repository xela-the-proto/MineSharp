using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Interfaces;
using Serilog;

namespace MineSharpAPI.Api;

public class Auth : IAuth
{
    /*
     * AUTENTICAZIONE
     */
    public async Task<IResult> Authenticate(DatabaseContext db, LoginBody inquilino,
        WebApplicationBuilder builder, HttpContext httpContext)
    {
        //Cerco nel db l'inquilino con l'email
        var body = db.User.First(s => s.Email == inquilino.email);
        if(Hashing.VerifyHash(inquilino.password, body.PasswordHash))
        {
            var issuer = builder.Configuration["Jwt:Issuer"];
            var audience = builder.Configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, inquilino.email),
                    new Claim(JwtRegisteredClaimNames.Email, inquilino.email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddDays(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokendescriptor);
            var stringToken = tokenHandler.WriteToken(token);

            //setup cookie
            httpContext.Response.Cookies.Append("jwt", stringToken, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            });

            return Results.Ok();
        }
        //Se verifico la sua hash allora proseguo ed emetto un token
        //Sennò ritorno 401
        return Results.Unauthorized();
    }
}