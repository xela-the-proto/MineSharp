using System.Data.SQLite;
using System.Text;
using MineSharpAPI.Modules.Interfaces;
using MineSharpAPI.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Bodies;
using MineSharpAPI.Queries;
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
        RegisterServices(builder);

        var app = builder.Build();

        
        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (context.Database.EnsureCreated())
            {
                Log.Warning("Database migrated and created");
            }

            if (!context.User.Any())
            { 
                var db = serviceScope.ServiceProvider.GetRequiredService<IDbUser>();
                db.SetUser(context, new LoginBody()
                {
                    email = "welcome@to.mineasharp",
                    password = "admin"
                });
            }

            context.SavedChanges += (sender, eventArgs) =>
            {

                Log.Warning($"Database saved {eventArgs.EntitiesSavedCount} entities");
            };
            context.SavingChanges += (sender, eventArgs) =>
            {
                Log.Warning("Syncyng db to EF queries");
            };
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

    public static void RegisterServices(WebApplicationBuilder builder)
    {
        var csb = new SQLiteConnectionStringBuilder();
        if (!File.Exists("Local.sqlite"))
        {
            csb.ConnectionString = "Data Source=" + Environment.CurrentDirectory + Path.DirectorySeparatorChar +"Local.sqlite";
            //builder.Configuration["ConnectionStrings:postgres_lin"] = csb.ConnectionString;
            SQLiteConnection.CreateFile(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Local.sqlite");
        }
        
         /*
         * Singletons
         */
        builder.Services.AddSingleton<IAuth, Auth>();
        builder.Services.AddScoped<IAuth, Auth>();
        builder.Services.AddSingleton<IDbUser, DbServer>();
        /*
         * Redis cache
         */
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["ConnectionStrings:Redis"];
        });
        /*
         * Logging
         */
        builder.Services.AddSerilog();
        /*
         * EF
         */
        builder.Services.AddDbContext<DatabaseContext>(opt =>
        {
            opt.UseSqlite(csb.ConnectionString).LogTo(Log.Debug).EnableDetailedErrors();

        });

        builder.WebHost.UseUrls("http://0.0.0.0:5000");
        
        //TODO: cors broken
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

       
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {   
            o.TokenValidationParameters = new TokenValidationParameters
            {
               
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["jwt"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
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
    }
}
