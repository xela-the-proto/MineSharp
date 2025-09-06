using Common.Process;
using Common.WebSocket;

namespace Runner.RunnerManager;

public class ServerRunner
{
    public bool HasExitedEvent = false;
    public void startServerProcess(List<string> flags ,List<string> values ,string workDir)
    {
        try
        {
            var process = ProcessInfoHelper.BuildStarterProcess("java", flags, values, workDir,
                true,true,true,false);
            Console.Clear();
            
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            var ws_thread = new Task(() => WebSocketServer.startWs(process,ct));
            //Thread ws_thread = new Thread(() => WebSocketServer.ServerStart(process, ct));
            
            Console.WriteLine("Start server");
            process.Start();
            Console.WriteLine("Start ws");
            ws_thread.Start();

            //TODO: Memory leak?
            while (!process.HasExited);
                
            Console.WriteLine("Cancelling");
            cts.Cancel();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}   