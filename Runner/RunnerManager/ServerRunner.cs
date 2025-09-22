using System.Buffers.Text;
using Common.Process;
using Common.WebSocket;
using Serilog;

namespace Runner.RunnerManager;

public class ServerRunner
{
    public void startServerProcess(List<string> args,string workdir)
    {
        try
        {
            Log.Verbose("Building process");
            var process = ProcessInfoHelper.BuildStarterProcess("java", args, workdir,
                true,true,true,false);
            
            Log.Verbose("Creating canc token");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            var ws_thread = new Task(() => 
                WebSocketServer.startWs(process,ct, Program.RUNNER_PROPERTIES.ShardGuid.ToString()));
            
            Log.Information("Start server");
            process.Start();
            Log.Information("Start ws");
            ws_thread.Start();
            
            //TODO: Memory leak?
            while (!process.HasExited);

            Log.Information("Cancelling");
            cts.Cancel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}