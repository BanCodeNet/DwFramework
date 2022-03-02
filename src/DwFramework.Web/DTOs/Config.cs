using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace DwFramework.Web;

public readonly record struct Config
{
    public readonly record struct Listen
    {
        public HttpProtocols Protocols { get; init; }
        public string Ip { get; init; }
        public int Port { get; init; }
        public string Cert { get; init; }
        public string Password { get; init; }
        public bool UseSSL { get; init; } = false;

        public Listen(HttpProtocols protocols, string ip, int port, string cert, string password, bool useSSl)
        {
            Protocols = protocols;
            Ip = ip;
            Port = port;
            Cert = cert;
            Password = password;
            UseSSL = useSSl;
        }
    }

    public readonly record struct Socket
    {
        public Listen Listen { get; init; }
        public int BufferSize { get; init; } = 1024 * 4;
        public int BackLog { get; init; } = 100;
        [JsonConverter(typeof(AddressFamily))]
        public AddressFamily AddressFamily { get; init; }
        [JsonConverter(typeof(SocketType))]
        public SocketType SocketType { get; init; }
        [JsonConverter(typeof(ProtocolType))]
        public ProtocolType ProtocolType { get; init; }

        public Socket(Listen listen, int bufferSize, int backLog, AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            Listen = listen;
            BufferSize = bufferSize;
            BackLog = backLog;
            AddressFamily = addressFamily;
            SocketType = socketType;
            ProtocolType = protocolType;
        }
    }

    public readonly record struct Http
    {
        public string ContentRoot { get; init; }
        public List<Listen> Listens { get; init; } = new();
        public int BufferSize { get; init; } = 1024 * 4;

        public Http(string contentRoot, List<Listen> listens, int bufferSize)
        {
            ContentRoot = contentRoot;
            Listens = listens;
            BufferSize = bufferSize;
        }
    }
}