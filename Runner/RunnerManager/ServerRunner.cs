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
            Thread ws_thread = new Thread(() => WebSocketServer.ServerStart(process, ct));
            
            Console.WriteLine("Start server");
            process.Start();
            Console.WriteLine("Start ws");
            ws_thread.Start();

            //TODO: Memory leak?
            while (!process.HasExited);
                
            Console.WriteLine("Cancelling");
            cts.Cancel();
        }
        catch (AggregateException e)
        {
            //We know that Cancel already throws because so we ignore
        }
    }
}   