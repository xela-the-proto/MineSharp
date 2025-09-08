using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using Common.Converters;
using WatsonWebsocket;

namespace Common.WebSocket;

public class WebSocketServer
{
    private static WatsonWsServer _server;
    private static Guid CLIENT_GUID;
    private static System.Diagnostics.Process SERVER_PROCESS;
    public static async Task startWs(System.Diagnostics.Process process, CancellationToken token)
    {
        try
        {
            WatsonWsServer server = new WatsonWsServer("127.0.0.1");
            _server = server;
            server.ClientConnected += OnServerClientConnected;
            server.MessageReceived += OnServerMessageReceived;
            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.BeginOutputReadLine();
            process.ErrorDataReceived += OnProcessErrorDataReceived;
            process.BeginErrorReadLine();
            
            SERVER_PROCESS = process;
            
            server.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs args)
    {
        _server.SendAsync(CLIENT_GUID, args.Data, WebSocketMessageType.Text, CancellationToken.None );
    }

    public static void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
    {
        _server.SendAsync(CLIENT_GUID, args.Data, WebSocketMessageType.Text, CancellationToken.None );
    }

    private static void OnServerMessageReceived(object? sender, MessageReceivedEventArgs args)
    {
        SERVER_PROCESS.StandardInput.WriteLine(Encoding.ASCII.GetString(args.Data));
    }

    private static void OnServerClientConnected(object? sender, ConnectionEventArgs args)
    {
        CLIENT_GUID = args.Client.Guid;
    }
}