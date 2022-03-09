using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;

namespace DwFramework.Web;

public sealed class WebSocketService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, WebSocketConnection> _connections = new();

    public int BufferSize { get; set; } = 1024 * 4;
    public event Func<WebSocketConnection, OnConnectEventArgs, Task> OnConnect;
    public event Func<WebSocketConnection, OnCloceEventArgs, Task> OnClose;
    public event Func<WebSocketConnection, OnSendEventArgs, Task> OnSend;
    public event Func<WebSocketConnection, OnReceiveEventArgs, Task> OnReceive;
    public event Func<WebSocketConnection, OnErrorEventArgs, Task> OnError;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration"></param>
    public WebSocketService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 创建连接
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task CreateConnectionAsync(HttpContext context)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        if (webSocket == null) return;
        var connection = new WebSocketConnection(
            webSocket,
            BufferSize,
            out var resetEvent,
            OnConnect,
            OnClose,
            OnSend,
            OnReceive,
            OnError
        );
        connection.OnClose += (c, a) =>
        {
            _connections.Remove(c.ID);
            return Task.CompletedTask;
        };
        _connections[connection.ID] = connection;
        _ = connection.BeginReceiveAsync();
        resetEvent.WaitOne();
    }

    /// <summary>
    /// 获取连接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public WebSocketConnection GetWebSocketConnection(string id)
        => _connections.ContainsKey(id) ? _connections[id] : null;

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public void BroadCast(byte[] data)
    {
        foreach (var item in _connections.Values)
        {
            _ = item.SendAsync(data);
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task CloseAsync(string id)
    {
        if (!_connections.ContainsKey(id)) return;
        var connection = _connections[id];
        await connection.CloseAsync(WebSocketCloseStatus.NormalClosure);
    }
}