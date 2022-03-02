namespace DwFramework.RabbitMQ;

public readonly record struct Config
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; }
    public string Password { get; init; }
    public string VirtualHost { get; init; } = "/";

    public Config(string host, int port, string userName, string password, string virtualHost)
    {
        Host = host;
        Port = port;
        UserName = userName;
        Password = password;
        VirtualHost = VirtualHost;
    }
}