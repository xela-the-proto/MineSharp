using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Common.Enums;
using MineSharpAPI.Modules.Helpers;
using Serilog;
using WatsonWebsocket;

namespace Common.WebSocket;

public class WebSocketServer
{
    private static WatsonWsServer? _server;
    public static int Port;
    private static RichCancellationToken? _cts;
    private static CancellationToken _ct;
    private static Guid _clientGuid;
    private static System.Diagnostics.Process? _serverProcess;

    public Task StartWs(System.Diagnostics.Process process, RichCancellationToken token)
    {
        Port = Random.Shared.Next(49152, 65535);
        try
        {
            Log.Information("Starting Watson ws");
            var server = new WatsonWsServer(port: Port);

            _cts = token;
            _ct = token.Token;
            _server = server;

            Log.Debug("Registering stuff");
            server.ClientConnected += OnServerClientConnected;
            server.MessageReceived += OnServerMessageReceived;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.BeginOutputReadLine();
            process.ErrorDataReceived += OnProcessErrorDataReceived;
            process.BeginErrorReadLine();

            _serverProcess = process;
            Log.Information("Websocket start");
            _server.StartAsync(_ct);
            while (!process.HasExited) ;
            Log.Debug($"Process {process.Id} exited");
            _cts.Cancel();
            Log.Debug("Sending cancelling signal");
            if (token.IsCancellationRequested)
            {
                Log.Warning("cancel + unsub");
                process.CancelOutputRead();
                process.CancelErrorRead();
                server.ClientConnected -= OnServerClientConnected;
                server.MessageReceived -= OnServerMessageReceived;
                process.OutputDataReceived -= OnProcessOutputDataReceived;
                process.ErrorDataReceived -= OnProcessErrorDataReceived;
            }

            Log.Warning("return");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }

        return Task.CompletedTask;
    }

    private static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs args)
    {
        try
        {
            if (!string.IsNullOrEmpty(args.Data))
                _server.SendAsync(_clientGuid, "[ERROR]Got null data!", WebSocketMessageType.Text, _ct);
            Console.WriteLine(args.Data);
            if (args.Data.Contains("For help, type \"help\""))
            {
                _cts.CurrentServerStatus = ServerStatus.RUNNING;
            }
        }
        catch (ArgumentNullException e)
        {
            Log.Warning("Got a null value when sending data down socket!, is the server shutting down?");
        }
    }

    private static void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
    {
        try
        {
            if (!string.IsNullOrEmpty(args.Data))
                _server.SendAsync(_clientGuid, "[ERROR]Got null data!", WebSocketMessageType.Text, _ct);
            
            Console.WriteLine(args.Data);
            
            if (args.Data.Contains("For help, type \"help\""))
            {
                _cts.CurrentServerStatus = ServerStatus.RUNNING;
            }
        }
        catch (ArgumentNullException e)
        {
            Log.Warning("Got a null value when sending data down socket!, is the server shutting down?");
        }
    }

    private static void OnServerMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        _serverProcess.StandardInput.WriteLine(Encoding.ASCII.GetString(args.Data));
#if DEBUG
        Console.WriteLine(args.Data);
#endif
    }

    private static void OnServerClientConnected(object? sender, ConnectionEventArgs args)
    {
        _clientGuid = args.Client.Guid;
    }
}