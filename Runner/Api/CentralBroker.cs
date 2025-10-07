using System.Diagnostics;
using Common.Enums;
using Common.WebSocket;
using Hardware.Info;
using MineSharpAPI.Modules.Bodies;
using Newtonsoft.Json;
using RestSharp;

namespace Runner.Api;


public class CentralBroker
{
    public static Task UpdateServerStatus(Process process, CancellationTokenSource cancellationToken)
    {
        HardwareInfo info = new HardwareInfo();
        using (var client = new RestClient("http://localhost:5000"))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                info.RefreshAll();
                var serverStats = new Server();
                serverStats.id = File.ReadAllText(Path.Combine(process.StartInfo.WorkingDirectory, "guid.txt"));
                serverStats.name = process.StartInfo.WorkingDirectory.Substring(
                    process.StartInfo.WorkingDirectory.IndexOf(Path.DirectorySeparatorChar)
                );
                serverStats.parentRunner = Program.RUNNER_PROPERTIES.ShardGuid.ToString();
                serverStats.status = ServerStatus.STOPPED;
                serverStats.usage = info.CpuList[0].CurrentClockSpeed;
                serverStats.wsPort = WebSocketServer._port;
                Thread.Sleep(3000);

                client.PutAsync(new RestRequest("/api/runners/updateServerStatus").AddBody(
                    JsonConvert.SerializeObject(serverStats))
                    .AddHeader("x-api-key",Program.RUNNER_PROPERTIES.token));
            }
        }
        return Task.CompletedTask;
    }
}