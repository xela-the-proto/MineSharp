using System.Net;
using Common.Json;
using Common.Process;
using Common.WebSocket;
using MineSharpAPI.Modules.Helpers;
using RestSharp;
using Runner.Api;
using Serilog;

namespace Runner.RunnerManager;

public class ServerRunner
{
    private static RichCancellationToken _cts;

    public void StartServerProcess(List<string> args, string workdir, bool eulaAccept)
    {
        Task wsThread;
        var broker = new CentralBroker();
        var ws = new WebSocketServer();
        var client = new RestClient("http://localhost:5000");
        try
        {
            Log.Verbose("Building process");
            var process = ProcessInfoHelper.BuildStarterProcess("java", args, workdir,
                true, true, true, false);
            
            var result  = client.ExecuteGetAsync<Server>(new RestRequest("/api/runners/getEulaStatus")
                .AddParameter("text/plain",File.ReadAllText(Path.Combine(process.StartInfo.WorkingDirectory,
                    "guid.txt")), ParameterType.RequestBody)
                .AddHeader("x-api-key", Program.RUNNER_PROPERTIES.token)).Result;

            Log.Verbose("Creating canc token");
            var cts = new RichCancellationToken();
            _cts = cts;
            
            if (result.StatusCode == HttpStatusCode.NotFound || result.Data.IsEulaAccepted == false)
            {
                process.Start();
                while (!process.HasExited);
                Log.Verbose("Exited first loop for eula, changing file content");
                var path = Path.Combine(workdir, "eula.txt");
                Log.Verbose("Opening file stream");
                var filestream = File.ReadAllText(path);
                var txt = filestream.Replace("eula=false", "eula=true");
                Log.Verbose("Write and close stream");
                File.WriteAllText(path, txt);
            }

            if (result.Data != null && result.Data.wsPort != 0)
            {
                wsThread = new Task(() =>
                    ws.StartWs(process, cts, result.Data.wsPort));
            }
            else
            {
                wsThread = new Task(() => ws.StartWs(process, cts));
            }
            var updateThread = new Task(() => broker.UpdateServerStatus(process, cts));

            Log.Debug("Start server");
            process.Start();
            Log.Debug("Start ws");
            wsThread.Start();
            Log.Debug("Start monitoring");
            updateThread.Start();
            

            while (!process.HasExited) ;

            Log.Information("Cancelling");
        }
        catch (ArgumentNullException e)
        {
            if (e.Message.Contains("Value cannot be null. (Parameter 'data')"))
            {
                Log.Warning("Received null as data to send down socket, is the server shutting down?");
                _cts.ExitReason = "ERR_SOCKET_STREAM";
                _cts.Cancel();
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            _cts.ExitReason = "ERR_GENERIC_RUNNER_ERROR";
            _cts.Cancel();
            throw;
        }
    }
}