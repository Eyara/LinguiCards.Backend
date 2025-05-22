namespace LinguiCards.Application.Common.Options;

public class RedisOptions
{
    public string Host { get; set; }
    public int Port { get; set; }

    public string GetConnectionString() => $"{Host}:{Port}";
}