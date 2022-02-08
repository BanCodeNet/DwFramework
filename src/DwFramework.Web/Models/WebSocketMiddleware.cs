using Microsoft.AspNetCore.Http;

namespace DwFramework.Web;

public sealed class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketService _webSocketService;

    public WebSocketMiddleware(RequestDelegate next, WebSocketService webSocketService)
    {
        _next = next;
        _webSocketService = webSocketService;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest) await _next(context);
        _ = _webSocketService.CreateConnectionAsync(context);
    }
}