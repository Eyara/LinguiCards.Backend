using System.Text;

namespace LinguiCards.Helpers;

public static class ProxyChainingHelper
{
    public static async Task SendConnect(Stream stream, string host, int port, string? username, string? password,
        CancellationToken token)
    {
        var authHeader = string.IsNullOrEmpty(username)
            ? ""
            : $"Proxy-Authorization: Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"))}\r\n";
        var connectRequest =
            $"CONNECT {host}:{port} HTTP/1.1\r\n" +
            $"Host: {host}:{port}\r\n" +
            authHeader +
            "\r\n";

        var buffer = Encoding.ASCII.GetBytes(connectRequest);
        await stream.WriteAsync(buffer, 0, buffer.Length, token);
        await stream.FlushAsync(token);

        using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
        string? line;
        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
            if (line.StartsWith("HTTP/1.1 200"))
                return;

        throw new Exception("Failed to establish CONNECT tunnel.");
    }
}