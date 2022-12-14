using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Mvc;
using ProtoBuf.Grpc.Server;
using System.Reflection;

namespace DwFramework.Web;

public static class ServiceHostExtension
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
    /// 添加RPC服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static IServiceCollection AddRpc(this IServiceCollection services, Action<GrpcServiceOptions> options = null)
    {
        services.AddCodeFirstGrpc(options);
        return services;
    }

    /// <summary>
    /// 添加Rpc实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static IEndpointRouteBuilder MapRpc<T>(this IEndpointRouteBuilder endpoint) where T : class
    {
        endpoint.MapGrpcService<T>();
        return endpoint;
    }

    /// <summary>
    /// 添加Rpc实现
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapRpc(this IEndpointRouteBuilder endpoint, Type[] rpcImpls)
    {
        var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
        foreach (var item in rpcImpls) method.MakeGenericMethod(item).Invoke(null, new object[] { endpoint });
        return endpoint;
    }

    /// <summary>
    /// 添加Rpc实现
    /// </summary>
    /// <param name="endpoint"></param>
    /// <param name="rpcImpl"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapRpc(this IEndpointRouteBuilder endpoint, Type rpcImpl)
        => endpoint.MapRpc(new[] { rpcImpl });

    /// <summary>
    /// 从程序集中添加Rpc实现
    /// </summary>
    /// <param name="endpoint"></param>
    public static IEndpointRouteBuilder MapRpcFromAssemblies(this IEndpointRouteBuilder endpoint)
    {
        var rpcImpls = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute<RpcAttribute>();
                if (attribute is null) continue;
                rpcImpls.Add(type);
            }
        }
        endpoint.MapRpc(rpcImpls.ToArray());
        return endpoint;
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