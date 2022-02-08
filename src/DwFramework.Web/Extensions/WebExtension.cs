using DwFramework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DwFramework.Web;

public static class WebExtension
{
    /// <summary>
    /// 配置Web主机
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ServiceHost ConfigureWebHost(this ServiceHost host, Action<IWebHostBuilder> configure)
    {
        host.ConfigureHostBuilder(builder => builder.ConfigureWebHost(configure));
        return host;
    }

    /// <summary>
    /// 配置Web主机
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static ServiceHost ConfigureWebHostDefaults(this ServiceHost host, Action<IWebHostBuilder> configure)
    {
        host.ConfigureHostBuilder(builder => builder.ConfigureWebHostDefaults(configure));
        return host;
    }

    /// <summary>
    /// 添加WebSocket中间件
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebSocket(this IServiceCollection services)
    {
        services.AddSingleton<WebSocketService>();
        return services;
    }

    /// <summary>
    /// 使用WebSocket中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseWebSocket(this IApplicationBuilder app)
    {
        app.UseWebSockets();
        app.UseMiddleware<WebSocketMiddleware>();
        return app;
    }

    /// <summary>
    /// 获取WebSocket服务
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static WebSocketService GetWebSocket(this IServiceProvider provider)
        => provider.GetService<WebSocketService>();

    /// <summary>
    /// 添加Rpc服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRpcImplements(this IServiceCollection services, params Type[] rpcImpls)
    {
        WebService.Instance.AddRpcImplements(services, rpcImpls);
        return services;
    }

    /// <summary>
    /// 匹配Rpc路由
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapRpcImplements(this IEndpointRouteBuilder endpoints)
    {
        WebService.Instance.MapRpcImplements(endpoints);
        return endpoints;
    }

    /// <summary>
    /// 使用全局路由前缀
    /// </summary>
    /// <param name="options"></param>
    /// <param name="prefix"></param>
    public static void UseRoutePrefix(this MvcOptions options, string prefix)
    {
        options.Conventions.Insert(0, new RoutePrefix(new RouteAttribute(prefix)));
    }
}