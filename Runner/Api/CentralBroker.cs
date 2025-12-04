using System.Diagnostics;
using Common.Converters;
using Common.Enums;
using Common.Json;
using Common.WebSocket;
using Hardware.Info;
using MineSharpAPI.Modules.Helpers;
using Newtonsoft.Json;
using RestSharp;
using Runner.DownloadManager;
using Runner.RunnerManager;

namespace Runner.Api;

public class CentralBroker
{

    public Task UpdateServerStatus(Process process, RichCancellationToken cancellationToken)
    {
        var serverStats = new Server();
        var info = new HardwareInfo();
        using (var client = new RestClient("http://localhost:5000"))
        {
            serverStats.ProcessId = process.Id;
            while (!cancellationToken.IsCancellationRequested)
            {
                info.RefreshAll();
               
                serverStats.id = File.ReadAllText(Path.Combine(process.StartInfo.WorkingDirectory, "guid.txt"));
                serverStats.name = process.StartInfo.WorkingDirectory.Substring(
                    process.StartInfo.WorkingDirectory.IndexOf(Path.DirectorySeparatorChar)
                );
                serverStats.parentRunner = Program.RUNNER_PROPERTIES.ShardGuid.ToString();
                serverStats.usage = info.CpuList[0].CurrentClockSpeed;
                serverStats.wsPort = WebSocketServer.Port;
                //If were at this point we are sure the eula got accepted
                serverStats.IsEulaAccepted = true;
                Thread.Sleep(3000);

                client.PutAsync(new RestRequest("/api/runners/updateServerStatus")
                    .AddBody(JsonConvert.SerializeObject(serverStats))
                    .AddHeader("x-api-key", Program.RUNNER_PROPERTIES.token),
                    cancellationToken.Token);
            }
            //If we exited many things could have happened
            //1. the socket errored out
            //2. the server stopped
            //3. the runner had an error
            //By extending the Cancellation Token to add some helpers 
            //we can use it as a way to communicate between all methods what the server is doing;

            switch (cancellationToken.ExitReason)
            {
                case "ERR_SOCKET_STREAM":
                    serverStats.status = ServerStatus.SOCKET_ERROR;
                    break;
                case "ERR_GENERIC_RUNNER_ERROR":
                    serverStats.status = ServerStatus.RUNNER_ERROR;
                    break;
            }
            client.PutAsync(new RestRequest("/api/runners/updateServerStatus").AddBody(
                    JsonConvert.SerializeObject(serverStats))
                .AddHeader("x-api-key", Program.RUNNER_PROPERTIES.token));
        }

        return Task.CompletedTask;
    }

    public void startServer(RunnerBody serverDetails)
    {
        var args = ArgsParser.BuildArgs(serverDetails);

        DownloadDispatch.DownloadJar(args[args.IndexOf("-v") + 1], args[args.IndexOf("-f") + 1]);
        var runner = new ServerRunner();

        runner.StartServerProcess(ConvertFlagsToJavaFlags.ConvertList(args), args[args.IndexOf("-f") + 1]);
    }
}