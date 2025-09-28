using System.Buffers.Text;
using Common.Process;
using Common.WebSocket;
using Serilog;

namespace Runner.RunnerManager;

public class ServerRunner
{
    private static CancellationTokenSource _cts;
    public void StartServerProcess(List<string> args,string workdir)
    {
        try
        {
            Log.Verbose("Building process");
            var process = ProcessInfoHelper.BuildStarterProcess("java", args, workdir,
                true, true, true, false);

            Log.Verbose("Creating canc token");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            _cts = cts;
            var ws_thread = new Task(() =>
                _ = WebSocketServer.StartWs(process, cts, Program.RUNNER_PROPERTIES.ShardGuid.ToString()));

            Log.Debug("Start server");
            process.Start();
            Log.Debug("Start ws");
            ws_thread.Start();

            //TODO: Memory leak?
            while (!process.HasExited) ;

            Log.Information("Cancelling");
        }
        catch (ArgumentNullException e)
        {
            Log.Debug("ArgumentNullExcpetion");
            if (e.Message.Contains("Value cannot be null. (Parameter 'data')"))
            {
                Log.Warning("Received null as data to send down socket, is the server shutting down?");
                _cts.Cancel();
            }
            else
            {
                throw;
            }
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            _cts.Cancel();
            throw;
        }
    }
}