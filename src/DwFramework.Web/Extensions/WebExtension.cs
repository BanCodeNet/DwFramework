using DwFramework.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Grpc.Configuration;
using System.Reflection;
using System.IO.Compression;

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
    /// 从程序集中注册Rpc服务
    /// </summary>
    private static void AddRpcFromAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RPCAttribute>();
                if (attribute == null) continue;
                AddRpc(type);
            }
        }
    }

    /// <summary>
    /// 添加RPC服务
    /// </summary>
    /// <param name="type"></param>
    private static void AddRpc(Type type)
    {
        var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
        var genericMethod = method.MakeGenericMethod(type);
        // _rpcImplBuild += endpoint => genericMethod.Invoke(null, new object[] { endpoint });
    }

    /// <summary>
    /// 添加RPC服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static void AddRpc<T>() => AddRpc(typeof(T));

    /// <summary>
    /// 添加Rpc服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="rpcImpls"></param>
    public static IServiceCollection AddRpc(this IServiceCollection services, params Type[] rpcImpls)
    {
        AddRpcFromAssemblies();
        foreach (var item in rpcImpls) AddRpc(item);
        services.AddCodeFirstGrpc(config => config.ResponseCompressionLevel = CompressionLevel.Optimal);
        services.AddSingleton(BinderConfiguration.Create(binder: new RpcServiceBinder(services)));
        services.AddCodeFirstGrpcReflection();
        return services;
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