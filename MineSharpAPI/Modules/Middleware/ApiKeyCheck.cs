using System.Data.Entity;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;

namespace MineSharpAPI.Modules.Middleware;

public class ApiKeyCheckMiddleware
{
    private readonly RequestDelegate _next;
    
    public ApiKeyCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task<IResult> InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("x-api-key") && context.Request.Path.Value.Contains("/api"))
        {
            var key =  context.Request.Headers["x-api-key"];
            var service = context.RequestServices.GetRequiredService<DatabaseContext>();
            var keyExist = service.ApiKeys.FirstOrDefault(x => x.Key == key[0]);
                
            if (!string.IsNullOrEmpty(key) && keyExist != null)
            { 
                context.User.
                await _next(context);      
            }else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Results.Unauthorized();
            } 
        }
        else
        {
            _next(context);
        }

        return null;
    }
}

public static class ApiKeyCheckMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyCheck(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyCheckMiddleware>();
    }
}