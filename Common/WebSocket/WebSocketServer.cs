using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Common.WebSocket;

public class WebSocketServer
{
    public static void ServerStart(System.Diagnostics.Process process, CancellationToken token)
    {
        try
        {
            var ip = "127.0.0.1";
            var port = 778;
            var server = new TcpListener(IPAddress.Parse(ip), port);

            token.Register(() =>
            {
                server.Stop();
                throw new OperationCanceledException();
            });

            server.Start();
            Console.WriteLine("Server has started on {0}:{1}, Waiting for a connection…", ip, port);

            var client = server.AcceptTcpClient();
            Console.WriteLine("A client connected.");
            var stream = client.GetStream();

            // --- Handshake ---
            var request = ReadHeaders(stream, token);
            Console.WriteLine("=====Handshaking from client=====\n{0}", request);

            var swk = ExtractHeader(request, "Sec-WebSocket-Key");
            var acceptKey = ComputeWebSocketAccept(swk);

            var response =
                "HTTP/1.1 101 Switching Protocols\r\n" +
                "Upgrade: websocket\r\n" +
                "Connection: Upgrade\r\n" +
                "Sec-WebSocket-Accept: " + acceptKey + "\r\n\r\n";

            var respBytes = Encoding.ASCII.GetBytes(response);
            stream.Write(respBytes, 0, respBytes.Length);

            Console.WriteLine("Handshake complete.");

            // --- After handshake, hook process output ---
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    SendWebSocketMessage(stream, e.Data);
            };
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    SendWebSocketMessage(stream, "[ERR] " + e.Data);
            };
            process.BeginErrorReadLine();

            // --- Main loop: read frames from client ---
            var buffer = new byte[8192];
            while (!token.IsCancellationRequested)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                var bytes = buffer.Take(bytesRead).ToArray();

                // parse WebSocket frame
                var mask = (bytes[1] & 0b10000000) != 0;
                ulong offset = 2, msgLen = (ulong)(bytes[1] & 0b01111111);

                if (msgLen == 126)
                {
                    msgLen = BitConverter.ToUInt16(new[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                }
                else if (msgLen == 127)
                {
                    msgLen = BitConverter.ToUInt64(
                        new[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0);
                    offset = 10;
                }

                if (msgLen > 0 && mask)
                {
                    var decoded = new byte[msgLen];
                    var masks = new byte[4]
                        { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (ulong i = 0; i < msgLen; ++i)
                        decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                    var text = Encoding.UTF8.GetString(decoded);
                    Console.WriteLine("Client sent: " + text);

                    // forward to process
                    process.StandardInput.WriteLine(text);
                }
            }
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10004)
                Console.WriteLine("Stopping accept incoming tcp connections for ws");
        }
    }

    // --- Helpers ---

    private static string ReadHeaders(NetworkStream stream, CancellationToken ct)
    {
        var sb = new StringBuilder();
        var buffer = new byte[1024];
        
        //Parse all headers until i get line breaks \r\n\r\n
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            int n = stream.Read(buffer, 0, buffer.Length);
            if (n <= 0) break;

            sb.Append(Encoding.ASCII.GetString(buffer, 0, n));
            if (sb.ToString().Contains("\r\n\r\n")) break;
        }

        return sb.ToString();
    }

    //Split headers inot theyr raw values without the type of header
    private static string ExtractHeader(string headers, string name)
    {
        foreach (var line in headers.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith(name + ":", StringComparison.OrdinalIgnoreCase))
                return line.Substring(name.Length + 1).Trim();
        }

        throw new InvalidOperationException($"Missing required header: {name}");
    }

    private static string ComputeWebSocketAccept(string key)
    {
        //guid based on FC 6455
        var combined = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(combined));
        return Convert.ToBase64String(hash);
    }

    private static void SendWebSocketMessage(NetworkStream stream, string message)
    {
        byte[] payload = Encoding.UTF8.GetBytes(message);
        List<byte> frame = new List<byte>();

        // FIN + text opcode
        frame.Add(0x81);

        if (payload.Length <= 125)
        {
            frame.Add((byte)payload.Length);
        }
        else if (payload.Length <= ushort.MaxValue)
        {
            frame.Add(126);
            frame.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)payload.Length)));
        }
        else
        {
            frame.Add(127);
            frame.AddRange(BitConverter.GetBytes((ulong)IPAddress.HostToNetworkOrder((long)payload.Length)));
        }

        frame.AddRange(payload);

        try
        {
            stream.Write(frame.ToArray(), 0, frame.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore invio messaggio WS: " + ex.Message);
        }
    }
}
