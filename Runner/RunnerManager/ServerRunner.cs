using Common.Process;
using Common.WebSocket;
using Runner.Api;
using Serilog;

namespace Runner.RunnerManager;

public class ServerRunner
{
    private static CancellationTokenSource _cts;

    public void StartServerProcess(List<string> args, string workdir, bool eulaAccept)
    {
        try
        {
            Log.Verbose("Building process");
            var process = ProcessInfoHelper.BuildStarterProcess("java", args, workdir,
                true, true, true, false);

            Log.Verbose("Creating canc token");
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            _cts = cts;
            if (eulaAccept)
            {
                process.Start();
                while (!process.HasExited) ;
                Log.Verbose("Exited first loop for eula, changing file content");
                var path = Path.Combine(workdir, "eula.txt");
                Log.Verbose("Opening file stream");
                var filestream = File.ReadAllText(path);
                var txt = filestream.Replace("eula=false", "eula=true");
                Log.Verbose("Write and close stream");
                File.WriteAllText(path, txt);
            }

            var wsThread = new Task(() =>
                WebSocketServer.StartWs(process, cts));
            var updateThread = new Task(() => CentralBroker.UpdateServerStatus(process, cts));

            Log.Debug("Start server");
            process.Start();
            Log.Debug("Start ws");
            wsThread.Start();
            Log.Debug("Start monitoring");
            updateThread.Start();


            //TODO: Memory leak?
            while (!process.HasExited) ;

            Log.Information("Cancelling");
        }
        catch (ArgumentNullException e)
        {
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