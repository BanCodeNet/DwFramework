using DwFramework.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;

namespace DwFramework.Web;

public sealed class WebSocketService
{
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, WebSocketConnection> _webSocketConnections = new();

    public event Action<WebSocketConnection, OnConnectEventArgs> OnWebSocketConnect;
    public event Action<WebSocketConnection, OnCloceEventArgs> OnWebSocketClose;
    public event Action<WebSocketConnection, OnSendEventArgs> OnWebSocketSend;
    public event Action<WebSocketConnection, OnReceiveEventArgs> OnWebSocketReceive;
    public event Action<WebSocketConnection, OnErrorEventArgs> OnWebSocketError;

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
        var config = _configuration.ParseConfiguration<Config.Web>();
        var connection = new WebSocketConnection(webSocket, config.BufferSize, out var resetEvent)
        {
            OnClose = OnWebSocketClose,
            OnSend = OnWebSocketSend,
            OnReceive = OnWebSocketReceive,
            OnError = OnWebSocketError
        };
        _webSocketConnections[connection.ID] = connection;
        OnWebSocketConnect?.Invoke(connection, new OnConnectEventArgs() { Header = context.Request.Headers });
        _ = connection.BeginReceiveAsync();
        resetEvent.WaitOne();
    }

    /// <summary>
    /// 获取连接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public WebSocketConnection GetWebSocketConnection(string id)
        => _webSocketConnections.ContainsKey(id) ? _webSocketConnections[id] : null;

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public void BroadCast(byte[] data)
    {
        foreach (var item in _webSocketConnections.Values)
        {
            _ = item.SendAsync(data);
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task CloseAsync(string id)
    {
        if (!_webSocketConnections.ContainsKey(id)) return Task.CompletedTask;
        var connection = _webSocketConnections[id];
        return connection.CloseAsync(WebSocketCloseStatus.NormalClosure);
    }
}