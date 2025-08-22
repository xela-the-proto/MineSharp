using System.Data.SQLite;
using System.Text;
using MineSharpAPI.Api;
using MineSharpAPI.Modules.Interfaces;
using MineSharpAPI.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Queries;
using MySqlConnector;
using Serilog;

public class program
{
    public static string runnerPath;
    public static void Main(string[] args)
    {
        
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.Development.json",true,true);

        //The path of the runner exe
        //TODO: MAKE THIS ALWAYSBE THE SAME THING   
        runnerPath = builder.Configuration["Paths:RunnerPath"];
        
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        var csb = new SQLiteConnectionStringBuilder(builder.Configuration["ConnectionStrings:postgres"]);
        if (csb.ConnectionString == "")
        {
            csb.ConnectionString = "Data Source=" + Environment.CurrentDirectory + @"\Local.sqlite";
            SQLiteConnection.CreateFile(Environment.CurrentDirectory +  @"\Local.sqlite");
            
        }
        
        /*
         * Singletons per poter accedere ad una singola istanza delle dependincy injection
         * https://learn.microsoft.com/it-it/dotnet/core/extensions/dependency-injection
         * https://refactoring.guru/design-patterns/singleton/csharp/example
         */
        builder.Services.AddSingleton<IAuth, Auth>();
        builder.Services.AddSingleton<IDbUser, DbServer>();
        /*
         * Connessionea redis per la cache
         */
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["ConnectionStrings:Redis"];
        });
        /*
         * Servizio del logger
         */
        builder.Services.AddSerilog();
        /*
         * Config per la orm con entity framework
         * https://learn.microsoft.com/it-it/ef/
         */
        builder.Services.AddDbContext<DatabaseContext>(opt =>
        {
            opt.UseSqlite(csb.ConnectionString).LogTo(Log.Debug).EnableDetailedErrors();
        });

        builder.WebHost.UseUrls("http://0.0.0.0:5000");
        //TODO: cors non funziona
        builder.Services.AddCors(options =>
        {
            //Frontend policy
            options.AddPolicy("Frontend", policyBuilder =>
            {
                policyBuilder.AllowAnyOrigin();
                policyBuilder.AllowAnyMethod();
                policyBuilder.AllowAnyHeader();
            });
        });

       
        builder.Services.AddAuthentication(options =>
        {
            //Prima abilito l'autenticazione tramite middelware
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            //Passo i parametri  per validare il token
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents
            {
                //Quando mi arriva una richiesta chiedo subito il cookie per confermare se è valido
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(token)) context.Token = token;
                    return Task.CompletedTask;
                }
            };
        });
        builder.Services.AddAuthorization();
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });


        /*
        builder.Services.AddRateLimiter(opt =>
        {
            opt.AddFixedWindowLimiter(policyName: "Restrictive", options =>
            {
                options.PermitLimit = 4;
                options.Window = TimeSpan.FromSeconds(10);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            });
        });
        */
        var app = builder.Build();
       
        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (context.Database.EnsureCreated())
            {
                context.Database.Migrate();
            }

            if (context.User.Count() == 0)
            { 
                var db = serviceScope.ServiceProvider.GetRequiredService<IDbUser>();
                db.SetUser(context, new LoginBody()
                {
                    email = "welcome@to.mineasharp",
                    password = "admin"
                });
            }
        }
      
        app.UseHttpsRedirection();
        Get.RegisterGets(app, builder);
        Put.RegisterPuts(app);
        Post.RegisterPosts(app);
        Delete.RegisterDeletes(app);

        app.UseCors("Frontend");
        app.UseAuthentication();
        app.UseAuthorization();
        //Middleware custom
        /*
        app.Use(async (con, next) =>
        {
        
            await next(con);
        });
        */
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature != null)
                {
                    var response = new { Message = "Errore interno del server" };
                    await context.Response.WriteAsJsonAsync(response);
                }
            });
        });

        //TODO: Profili ratelimiter
        //app.UseRateLimiter();
        app.UseResponseCompression();
        app.Run();
    }
}
