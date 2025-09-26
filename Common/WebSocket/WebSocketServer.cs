using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Common.Converters;
using Serilog;
using WatsonWebsocket;

namespace Common.WebSocket;

public class WebSocketServer
{
    private static WatsonWsServer _server;
    private static CancellationTokenSource _cts;
    private static CancellationToken _ct;
    private static Guid CLIENT_GUID;
    private static System.Diagnostics.Process SERVER_PROCESS;
    public static Task StartWs(System.Diagnostics.Process process, CancellationTokenSource token, string guid)
    {
        try
        {
            Log.Information("Starting Watson ws");
            WatsonWsServer server = new WatsonWsServer();

            _cts = token;
            _ct = token.Token;
            _server = server;

            Log.Information("Registering stuff");
            server.ClientConnected += OnServerClientConnected;
            server.MessageReceived += OnServerMessageReceived;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.BeginOutputReadLine();
            process.ErrorDataReceived += OnProcessErrorDataReceived;
            process.BeginErrorReadLine();

            SERVER_PROCESS = process;
            Log.Information("Websocket start");
            _server.StartAsync(_ct);

            while (!process.HasExited)
            {
            };
            Log.Warning("exit");
            _cts.Cancel();
            Log.Warning("Cancel");
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
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            throw;
        }

        return Task.CompletedTask;
    }

    public static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs args)
    {
        try
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                _server.SendAsync(CLIENT_GUID, "[ERROR]Got null data!", WebSocketMessageType.Text, _ct);

            }
            Console.WriteLine(args.Data);
        }
        catch (ArgumentNullException e)
        {
            Log.Warning($"Got a null value when sending data down socket!, is the server shutting down?");
        }
    }

    public static void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
    {
        try
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                _server.SendAsync(CLIENT_GUID, "[ERROR]Got null data!", WebSocketMessageType.Text, _ct);
            }
            Console.WriteLine(args.Data);
        }
        catch (ArgumentNullException e)
        {
            Log.Warning($"Got a null value when sending data down socket!, is the server shutting down?");
        }
    }

    private static void OnServerMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        SERVER_PROCESS.StandardInput.WriteLine(Encoding.ASCII.GetString(args.Data));
        #if DEBUG
        Console.WriteLine(args.Data);
        #endif
    }

    private static void OnServerClientConnected(object? sender, ConnectionEventArgs args)
    {
        CLIENT_GUID = args.Client.Guid;
    }
}