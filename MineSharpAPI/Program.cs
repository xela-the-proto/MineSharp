using System.Text;
using Common.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Hashing;
using MineSharpAPI.Modules.Interfaces;
using MineSharpAPI.Modules.Middleware;
using MineSharpAPI.Routes;
using Newtonsoft.Json.Linq;
using Npgsql;
using RestSharp;
using Serilog;

public class Program
{
    public static string splash =
        "$$\\      $$\\ $$$$$$\\ $$\\   $$\\ $$$$$$$$\\  $$$$$$\\  $$\\   $$\\  $$$$$$\\  $$$$$$$\\  $$$$$$$\\  \n$$$\\    $$$ |\\_$$  _|$$$\\  $$ |$$  _____|$$  __$$\\ $$ |  $$ |$$  __$$\\ $$  __$$\\ $$  __$$\\ \n$$$$\\  $$$$ |  $$ |  $$$$\\ $$ |$$ |      $$ /  \\__|$$ |  $$ |$$ /  $$ |$$ |  $$ |$$ |  $$ |\n$$\\$$\\$$ $$ |  $$ |  $$ $$\\$$ |$$$$$\\    \\$$$$$$\\  $$$$$$$$ |$$$$$$$$ |$$$$$$$  |$$$$$$$  |\n$$ \\$$$  $$ |  $$ |  $$ \\$$$$ |$$  __|    \\____$$\\ $$  __$$ |$$  __$$ |$$  __$$< $$  ____/ \n$$ |\\$  /$$ |  $$ |  $$ |\\$$$ |$$ |      $$\\   $$ |$$ |  $$ |$$ |  $$ |$$ |  $$ |$$ |      \n$$ | \\_/ $$ |$$$$$$\\ $$ | \\$$ |$$$$$$$$\\ \\$$$$$$  |$$ |  $$ |$$ |  $$ |$$ |  $$ |$$ |      \n\\__|     \\__|\\______|\\__|  \\__|\\________| \\______/ \\__|  \\__|\\__|  \\__|\\__|  \\__|\\__|      \n                                                                                           \n                                                                                           \n                                                                                           ";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.Development.json", true, true);


        Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();
        Log.Information("\n" + splash);
        RegisterServices(builder);

        var app = builder.Build();


        using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            var factory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<DatabaseContext>>();

            var context = factory.CreateDbContext();
            context.Database.Migrate();
            Log.Warning("Database migrated and created");

            if (!context.User.Any())
            {
                context.User.Add(new User
                {
                    Email = "welcome@to.mineasharp",
                    Id = "1",
                    PasswordHash = HashingUtils.HashString("admin") 
                });
            }

            if (!context.ApiKeys.Any())
            {
                var auth = serviceScope.ServiceProvider.GetRequiredService<IAuth>();

                context.ApiKeys.Add(new APIKeys
                {
                    Key = auth.GenApiKey(),
                    keyName = "MASTER_KEY",
                    OwnerID = "0"
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

            context.SaveChanges();
        }

        app.UseHttpsRedirection();
        Get.RegisterGets(app, builder);
        Put.RegisterPuts(app);
        Post.RegisterPosts(app);
        Delete.RegisterDeletes(app);

        app.UseCors("AllowFrontend");


        app.UseAuthentication();
        app.UseAuthorization();
        app.UseApiKeyCheck();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                JToken? token = null; 
                using (var client = new RestClient("https://httpducks.com/")) 
                { 
                    var address = client.GetAsync(new RestRequest("/404.json")).Result; 
                    var jobjetc = JObject.Parse(address.Content); 
                    token = jobjetc.SelectToken("image.webp"); 
                    var httpclient = new HttpClient();
                }
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/html";

                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (errorFeature != null)
                {
                    var response = new { Message = "Errore interno del server" };
                    await context.Response.WriteAsync($"<img source={token}>");
                }
            });
        });

        //TODO: Profili ratelimiter
        //app.UseRateLimiter();
        app.UseResponseCompression();
        app.Run("http://localhost:5000/");
    }

    public static void RegisterServices(WebApplicationBuilder builder)
    {
        /*
         * Singletons
         */
        builder.Services.AddSingleton<IAuth, Auth>();
        builder.Services.AddScoped<IAuth, Auth>();
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
       

        builder.Services.AddPooledDbContextFactory<DatabaseContext>(opt =>
        {
            var conn = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost:5432",
                Username = "postgres",
                Password =  builder.Configuration["ConnectionStrings:postgres"]
            };

            opt.UseNpgsql(conn.ConnectionString);
            //opt.usenp(csb.ConnectionString).LogTo(Log.Debug).EnableDetailedErrors();
        });
        
        builder.WebHost.UseUrls("http://0.0.0.0:5000");

        //TODO: cors broken
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    policy.SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });



        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
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
                        if (!string.IsNullOrEmpty(token)) context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddAuthorization(options => { });
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });
        builder.Services.AddAutoMapper(cfg => {
            cfg.LicenseKey = builder.Configuration["Automapper:Key"];
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