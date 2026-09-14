namespace Arphix;

public class HttpRequest
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";

    public static HttpRequest Parse(string rawRequest)
    {
        string[] lines = rawRequest.Split('\r', '\n');
        if (lines.Length == 0) return new HttpRequest();
        string[] parts = lines[0].Split(' ');
        return new HttpRequest
        {
            Method = parts.Length > 0 ? parts[0] : "GET",
            Path = parts.Length > 1 ? parts[1] : "/"
        };
    }
}
