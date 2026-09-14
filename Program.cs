using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Arphix;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Initialize Folders
        Directory.CreateDirectory("Data/Rooms");
        Directory.CreateDirectory("Data/Avatar");

        // 2. Load Config
        if (!File.Exists("config.json"))
            File.WriteAllText("config.json", "{\"Port\": 11000}");
        
        var config = JObject.Parse(File.ReadAllText("config.json"));
        int port = config["Port"]?.Value<int>() ?? 11000;

        // 3. Start Listener
        TcpListener server = new(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"[ARPHIX] Server online on port {port}.");

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        byte[] buffer = new byte[8192];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        
        string raw = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        var req = HttpRequest.Parse(raw);
        string body = "{}";

        // 4. Routing
        if (req.Path.StartsWith("/api/accounts/")) 
            body = "{\"accountId\": 1, \"username\": \"ArphixUser\"}";
        else if (req.Path.StartsWith("/api/rooms/")) 
            body = File.Exists("Data/Rooms/1.json") ? File.ReadAllText("Data/Rooms/1.json") : "{\"status\":\"no_room\"}";
        else if (req.Path.StartsWith("/api/avatar/")) 
            body = "{\"items\": []}";

        // 5. Respond
        string response = $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {body.Length}\r\nConnection: close\r\n\r\n{body}";
        await stream.WriteAsync(Encoding.UTF8.GetBytes(response));
        client.Close();
    }
}
