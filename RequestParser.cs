namespace Arphix;

public class HttpRequest
{
    public string Method { get; set; } = "";
    public string Path { get; set; } = "";

    public static HttpRequest Parse(string rawRequest)
    {
        string[] lines = rawRequest.Split('\r', '\n');
        string[] parts = lines[0].Split(' ');
        return new HttpRequest
        {
            Method = parts[0],
            Path = parts.Length > 1 ? parts[1] : "/"
        };
    }
}
