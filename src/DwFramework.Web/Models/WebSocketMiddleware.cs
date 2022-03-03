using Microsoft.AspNetCore.Http;

namespace DwFramework.Web;

public sealed class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketService _webSocketService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="next"></param>
    /// <param name="webSocketService"></param>
    public WebSocketMiddleware(RequestDelegate next, WebSocketService webSocketService)
    {
        _next = next;
        _webSocketService = webSocketService;
    }

    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest) await _next(context);
        _ = _webSocketService.CreateConnectionAsync(context);
    }
}