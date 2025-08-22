using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

            // Setup async read from process stdout
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) SendWebSocketMessage(stream, e.Data);
            };
            process.BeginOutputReadLine();

            // (Opzionale) leggi anche stderr
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) SendWebSocketMessage(stream, "[ERR] " + e.Data);
            };
            process.BeginErrorReadLine();

            // enter to an infinite cycle to be able to handle every change in stream
            while (!token.IsCancellationRequested)
            {
                while (!stream.DataAvailable && !token.IsCancellationRequested) ;
                while (client.Available < 3 && !token.IsCancellationRequested) ; // match against "get"

                var bytes = new byte[client.Available];
                stream.Read(bytes, 0, bytes.Length);
                var s = Encoding.UTF8.GetString(bytes);

                if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                    var swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                    var swkAndSalt = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                    var swkAndSaltSha1 = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swkAndSalt));
                    var swkAndSaltSha1Base64 = Convert.ToBase64String(swkAndSaltSha1);

                    var response = Encoding.UTF8.GetBytes(
                        "HTTP/1.1 101 Switching Protocols\r\n" +
                        "Connection: Upgrade\r\n" +
                        "Upgrade: websocket\r\n" +
                        "Sec-WebSocket-Accept: " + swkAndSaltSha1Base64 + "\r\n\r\n");

                    stream.Write(response, 0, response.Length);
                }
                else
                {
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
                            new[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] },
                            0);
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

                        // Scrivi input nel processo
                        process.StandardInput.WriteLine(text);
                    }
                }
            }
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10004) Console.WriteLine("Stopping accept incoming tcp connections for ws");
        }
    }

    private static void SendWebSocketMessage(NetworkStream stream, string message)
    {
        byte[] payload = Encoding.UTF8.GetBytes(message);
        List<byte> frame = new List<byte>();

        // primo byte: FIN + opcode (text)
        frame.Add(0x81);

        if (payload.Length <= 125)
        {
            frame.Add((byte)payload.Length); // MASK=0, len direttamente
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