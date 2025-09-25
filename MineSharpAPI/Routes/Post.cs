using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Common.Process;
using Microsoft.AspNetCore.Http.HttpResults;
using MineSharpAPI.Modules.Bodies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MineSharpAPI.Modules.Api;
using MineSharpAPI.Modules.Interfaces;
using PusherServer;
using RestSharp;
using Runner;
using Serilog;

namespace MineSharpAPI.Routes;

public class Post
{
    public static void RegisterPosts(WebApplication app)
    {
        app.MapPost("/api/Runners/RunServer", async ([FromBody]RunnerBody body, HttpContext context, DatabaseContext db) =>
        {
            
            using (var client = new RestClient(body.remoteUrl))
            {
                await client.PostAsync(new RestRequest("/startServer", Method.Post).AddBody(body));
            }
            /*
            var runner = new Process();
            runner.StartInfo.FileName = program.runnerPath;
            runner.StartInfo.Arguments = "-v " + body.version + " -f " + body.path + " -r " + body.ram ;
            runner.StartInfo.CreateNoWindow = false;
            runner.StartInfo.UseShellExecute = true;
            runner.Start();
            */
        });
        
        app.MapPost("/api/Runners/CreateServer", async ([FromBody]RunnerBody body, HttpContext context, DatabaseContext db) =>
        {
            
        });
        
        app.MapPost("/api/runners/GenAPIToken", async (HttpContext http, DatabaseContext db) =>
        {
            var result = Tokens.CreateApiToken(http, db);
            return result.Result;
        }).RequireAuthorization();
        
        
    }
}