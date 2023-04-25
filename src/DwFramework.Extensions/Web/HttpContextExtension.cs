using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DwFramework.Extensions;

public static class HttpContextExtension
{
    /// <summary>
    /// 获取Claims
    /// </summary>
    /// <param name="context"></param>
    /// <param name="claimType"></param>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static bool TryGetClaims(this HttpContext context, string claimType, out Claim[] claims)
    {
        claims = context.User.FindAll(claimType).ToArray();
        if (claims.Length <= 0) return false;
        return true;
    }

    /// <summary>
    /// 获取Claims
    /// </summary>
    /// <param name="context"></param>
    /// <param name="claimType"></param>
    /// <returns></returns>
    public static Claim[] GetNameIdentifiers(this HttpContext context, string claimType)
    {
        var claims = context.User.FindAll(claimType).ToArray();
        if (claims.Length <= 0) throw new Exception("invaild token");
        return claims;
    }

    /// <summary>
    /// 初始化SSE环境
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task InitSSEAsync(this HttpContext context)
    {
        context.Response.Headers.Add("Content-Type", "text/event-stream");
        context.Response.Headers.Add("Cache-Control", "no-cache");
        context.Response.Headers.Add("Connection", "keep-alive");
        await context.Response.Body.FlushAsync();
    }

    /// <summary>
    /// 发送EventStream事件
    /// </summary>
    public static async Task SendSSEEventAsync(this HttpContext context, EventStreamEvent @event)
    {
        context.Response.Headers.Add("Content-Type", "text/event-stream");
        context.Response.Headers.Add("Cache-Control", "no-cache");
        context.Response.Headers.Add("Connection", "keep-alive");
        await context.Response.Body.FlushAsync();
        await context.Response.WriteAsync($"id: {@event.Id}\n");
        await context.Response.WriteAsync($"event: {@event.Event}\n");
        if (@event.Retry is not null) await context.Response.WriteAsync($"retry: {@event.Retry}\n");
        if (@event.Data is not null) await foreach (var item in @event.Data) await context.Response.WriteAsync($"data: {item.ToJson()}\n\n");
        await context.Response.Body.FlushAsync();
    }
}