namespace LinguiCards.Application.Common.Options;

public class ProxyOptions
{
    public int ChainLength { get; set; } = 2;
    public ProxyAddress AddressA { get; set; } = null!;
    public ProxyAddress AddressB { get; set; } = null!;
}

public class ProxyAddress
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}