﻿using DwFramework.Core;
using System.Net.WebSockets;

namespace DwFramework.Web;

public sealed class WebSocketClient
{
    public event Action<OnConnectEventArgs> OnConnect;
    public event Action<OnCloceEventArgs> OnClose;
    public event Action<OnSendEventArgs> OnSend;
    public event Action<OnReceiveEventArgs> OnReceive;
    public event Action<OnErrorEventArgs> OnError;

    private readonly ClientWebSocket _client;
    private int _bufferSize;
    public int BufferSize
    {
        get => _bufferSize;
        set { if (value > 0) _bufferSize = value; }
    }
    public WebSocketState State => _client.State;

    /// <summary>
    /// 构造函数
    /// </summary>
    public WebSocketClient(int bufferSize = 4096)
    {
        _bufferSize = bufferSize;
        _client = new ClientWebSocket();
    }

    /// <summary>
    /// 建立连接
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="header"></param>
    /// <param name="subProtocal"></param>
    /// <returns></returns>
    public async Task ConnectAsync(string uri, Dictionary<string, string> header = null, List<string> subProtocal = null)
    {
        if (header != null) foreach (var item in header) _client.Options.SetRequestHeader(item.Key, item.Value);
        if (subProtocal != null) foreach (var item in subProtocal) _client.Options.AddSubProtocol(item);
        await _client.ConnectAsync(new Uri(uri), CancellationToken.None).ContinueWith(a =>
        {
            if (_client.State != WebSocketState.Open) throw new ExceptionBase(ExceptionType.Internal, 0, "无法连接");
            OnConnect?.Invoke(new OnConnectEventArgs() { });
            Task.Run(async () =>
            {
                var buffer = new byte[_bufferSize];
                var dataBytes = new List<byte>();
                WebSocketCloseStatus? closeStates = null;
                while (_client.State == WebSocketState.Open)
                {
                    try
                    {
                        var result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.CloseStatus.HasValue)
                        {
                            closeStates = result.CloseStatus;
                            break;
                        }
                        dataBytes.AddRange(buffer.Take(result.Count));
                        if (!result.EndOfMessage) continue;
                        OnReceive?.Invoke(new OnReceiveEventArgs() { Data = dataBytes.ToArray() });
                        dataBytes.Clear();
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(new OnErrorEventArgs() { Exception = ex });
                        continue;
                    }
                }
                OnClose?.Invoke(new OnCloceEventArgs() { });
                ClearAllEvent();
                if (_client.State == WebSocketState.CloseReceived) await CloseAsync();
            });
        });
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public async Task SendAsync(byte[] buffer)
    {
        await _client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully) OnSend?.Invoke(new OnSendEventArgs() { Data = buffer });
            else OnError?.Invoke(new OnErrorEventArgs() { Exception = task.Exception.InnerException });
        });
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    }

    /// <summary>
    /// 清空事件列表
    /// </summary>
    private void ClearAllEvent()
    {
        if (OnConnect != null)
        {
            var actions = OnConnect.GetInvocationList();
            foreach (var item in actions) OnConnect -= (Action<OnConnectEventArgs>)item;
        }
        if (OnClose != null)
        {
            var actions = OnClose.GetInvocationList();
            foreach (var item in actions) OnClose -= (Action<OnCloceEventArgs>)item;
        }
        if (OnError != null)
        {
            var actions = OnError.GetInvocationList();
            foreach (var item in actions) OnError -= (Action<OnErrorEventArgs>)item;
        }
        if (OnSend != null)
        {
            var actions = OnSend.GetInvocationList();
            foreach (var item in actions) OnSend -= (Action<OnSendEventArgs>)item;
        }
        if (OnReceive != null)
        {
            var actions = OnReceive.GetInvocationList();
            foreach (var item in actions) OnReceive -= (Action<OnReceiveEventArgs>)item;
        }
    }
}