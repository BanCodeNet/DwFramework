using DwFramework.Core;
using System.Net.WebSockets;

namespace DwFramework.Web;

public sealed class WebSocketConnection
{
    public string ID { get; init; }
    public bool IsClose { get; private set; } = false;

    private readonly WebSocket _webSocket;
    private readonly byte[] _buffer;
    private readonly List<byte> _dataBytes = new();
    private readonly AutoResetEvent _resetEvent;

    public event Func<WebSocketConnection, OnConnectEventArgs, Task> OnConnect;
    public event Func<WebSocketConnection, OnCloceEventArgs, Task> OnClose;
    public event Func<WebSocketConnection, OnSendEventArgs, Task> OnSend;
    public event Func<WebSocketConnection, OnReceiveEventArgs, Task> OnReceive;
    public event Func<WebSocketConnection, OnErrorEventArgs, Task> OnError;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="bufferSize"></param>
    /// <param name="resetEvent"></param>
    /// <param name="onConnect"></param>
    /// <param name="onClose"></param>
    /// <param name="onSend"></param>
    /// <param name="onReceive"></param>
    /// <param name="onError"></param>
    public WebSocketConnection(
        WebSocket webSocket,
        int bufferSize,
        out AutoResetEvent resetEvent,
        Func<WebSocketConnection, OnConnectEventArgs, Task> onConnect = null,
        Func<WebSocketConnection, OnCloceEventArgs, Task> onClose = null,
        Func<WebSocketConnection, OnSendEventArgs, Task> onSend = null,
        Func<WebSocketConnection, OnReceiveEventArgs, Task> onReceive = null,
        Func<WebSocketConnection, OnErrorEventArgs, Task> onError = null
    )
    {
        ID = Guid.NewGuid().ToString();
        _webSocket = webSocket;
        _buffer = new byte[bufferSize > 0 ? bufferSize : 4096];
        _resetEvent = new AutoResetEvent(false);
        resetEvent = _resetEvent;
        OnConnect += onConnect;
        OnClose += onClose;
        OnSend += onSend;
        OnReceive += onReceive;
        OnError += onError;
        OnConnect?.Invoke(this, new OnConnectEventArgs() { });
    }

    /// <summary>
    /// 开始接收数据
    /// </summary>
    /// <returns></returns>
    public async Task BeginReceiveAsync()
    {
        try
        {
            if (IsClose) return;
            if (_webSocket.State != WebSocketState.Open) throw new WebSocketException("连接状态异常");
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);
            if (result.CloseStatus.HasValue)
            {
                if (_webSocket.State == WebSocketState.CloseReceived && !IsClose)
                    await CloseAsync(result.CloseStatus.Value);
                return;
            }
            _dataBytes.AddRange(_buffer.Take(result.Count));
            if (result.EndOfMessage)
            {
                OnReceive?.Invoke(this, new OnReceiveEventArgs() { Data = _dataBytes.ToArray() });
                _dataBytes.Clear();
            }
            await BeginReceiveAsync();
        }
        catch (WebSocketException ex)
        {
            switch (ex.ErrorCode)
            {
                // TODO
                default:
                    OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                    await CloseAsync(WebSocketCloseStatus.InternalServerError);
                    break;
            }
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
            await BeginReceiveAsync();
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public async Task SendAsync(byte[] buffer)
    {
        try
        {
            if (_webSocket.State != WebSocketState.Open) throw new ExceptionBase(ExceptionType.Internal, 0, "连接状态异常");
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            OnSend?.Invoke(this, new OnSendEventArgs() { Data = buffer });
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="closeStatus"></param>
    /// <returns></returns>
    public async Task CloseAsync(WebSocketCloseStatus closeStatus)
    {
        if (IsClose) return;
        if (_webSocket.State == WebSocketState.Open)
            await _webSocket.CloseOutputAsync(closeStatus, null, CancellationToken.None);
        _webSocket.Abort();
        _webSocket.Dispose();
        IsClose = true;
        OnClose?.Invoke(this, new OnCloceEventArgs() { });
        _resetEvent.Set();
    }
}