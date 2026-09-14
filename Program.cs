using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Arphix;

class Program
{
    private static readonly TcpListener _server = new(IPAddress.Any, 11000);

    static async Task Main(string[] args)
    {
        InitializeEnvironment();
        _server.Start();
        Console.WriteLine("[ARPHIX] Server Initialized on Port 11000");

        while (true)
        {
            var client = await _server.AcceptTcpClientAsync();
            _ = HandleRequest(client);
        }
    }

    static void InitializeEnvironment()
    {
        Directory.CreateDirectory("Data/Rooms");
        Directory.CreateDirectory("Data/Avatar");
        
        string defaultRoom = "{\"RoomId\":\"1\", \"Name\":\"RecCenter\", \"Data\":{}}";
        if (!File.Exists("Data/Rooms/1.json")) File.WriteAllText("Data/Rooms/1.json", defaultRoom);
    }

    static async Task HandleRequest(TcpClient client)
    {
        using var stream = client.GetStream();
        byte[] buffer = new byte[8192];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        string route = request.Split(' ')[1];
        string jsonResponse = "{\"status\":\"error\"}";

        // Route mapping based on RR revival standards
        if (route.StartsWith("/api/rooms/v1/"))
        {
            jsonResponse = File.ReadAllText("Data/Rooms/1.json");
        }
        else if (route.StartsWith("/api/avatar/v1/items"))
        {
            jsonResponse = "{\"items\": [{\"Id\":\"shirt_01\", \"Name\":\"Basic\"}]}";
        }

        string response = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {jsonResponse.Length}\r\nConnection: close\r\n\r\n{jsonResponse}";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
        client.Close();
    }
}
