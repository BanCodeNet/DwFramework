using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;

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
    /// <param name="host"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static ServiceHost AddRpc(this ServiceHost host, Action<GrpcServiceOptions> options = null)
    {
        host.ConfigureServices(services => services.AddCodeFirstGrpc(options));
        return host;
    }
}