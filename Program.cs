using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Arphix;

class Program
{
    private static readonly TcpListener _server = new(IPAddress.Any, 11000);

    static async Task Main(string[] args)
    {
        InitializeEnvironment();
        _server.Start();
        Console.WriteLine("[ARPHIX] Core Online. Ready for Handshake.");

        while (true)
        {
            TcpClient client = await _server.AcceptTcpClientAsync();
            _ = HandleRequest(client);
        }
    }

    static void InitializeEnvironment()
    {
        Directory.CreateDirectory("Data/Rooms");
        Directory.CreateDirectory("Data/Avatar");
    }

    static async Task HandleRequest(TcpClient client)
    {
        using var stream = client.GetStream();
        byte[] buffer = new byte[8192];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        if (bytesRead == 0) return;

        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        string[] lines = request.Split('\r', '\n');
        string route = lines[0].Split(' ')[1];

        string responseBody = "{}";

        // Handshake and Auth routes
        if (route.Contains("/api/accounts/v1/")) responseBody = "{\"accountId\": 1, \"username\": \"ArphixUser\"}";
        else if (route.Contains("/api/rooms/v1/")) responseBody = File.Exists("Data/Rooms/1.json") ? File.ReadAllText("Data/Rooms/1.json") : "{\"RoomId\":1}";
        else if (route.Contains("/api/avatar/v1/")) responseBody = "{\"items\": []}";

        string response = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {responseBody.Length}\r\nConnection: close\r\n\r\n{responseBody}";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
        client.Close();
    }
}
