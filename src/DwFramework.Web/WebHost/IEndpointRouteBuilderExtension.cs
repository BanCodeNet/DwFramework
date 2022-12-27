using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace DwFramework.Web;

public static class IEndpointRouteBuilderExtension
{

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
    public static IEndpointRouteBuilder MapRpc(this IEndpointRouteBuilder endpoint, IEnumerable<Type> rpcImpls)
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
    {
        endpoint.MapRpc(new[] { rpcImpl });
        return endpoint;
    }

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
        endpoint.MapRpc(rpcImpls);
        return endpoint;
    }
}