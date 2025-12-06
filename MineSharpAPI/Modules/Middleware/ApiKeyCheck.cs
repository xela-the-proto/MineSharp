using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;
using Serilog;

namespace MineSharpAPI.Modules.Middleware;

public class ApiKeyCheckMiddleware
{
    private readonly RequestDelegate _next;
    private static DatabaseContext _db; 

    public ApiKeyCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task<IResult> InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.Request.Headers.ContainsKey("x-api-key") && context.Request.Path.Value.Contains("/api"))
            {
                var key = context.Request.Headers["x-api-key"];
                var service = context.RequestServices.GetRequiredService<IDbContextFactory<DatabaseContext>>();
                if (_db == null)
                {
                    _db = service.CreateDbContext();
                }
                var keyExist = _db.ApiKeys.FirstOrDefaultAsync(x => x.Key == key[0]);

                if (!string.IsNullOrEmpty(key) && keyExist.Result != null)
                {
                    Log.Verbose("Good api key");
                    await _next(context);
                }
                else
                {
                    Log.Verbose("Bad api key");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Results.Unauthorized();
                }
            }
            else if (!context.Request.Headers.ContainsKey("x-api-key") && context.Request.Path.Value.Contains("/api"))
            {
                Log.Verbose("Missing api key header");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Results.Unauthorized();
            }
            else
            {
                Log.Verbose("no auth");
                _next(context);
            }
        }
        catch (Exception e)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var bodystream = new StreamWriter(context.Response.Body);
            bodystream.Write("Pipeline failure!");
            Log.Fatal(e.Message);
            return Results.InternalServerError();
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