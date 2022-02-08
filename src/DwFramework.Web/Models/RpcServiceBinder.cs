using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;
using System.Reflection;

namespace DwFramework.Web;

public sealed class RpcServiceBinder : ServiceBinder
{
    private readonly IServiceCollection _services;

    public RpcServiceBinder(IServiceCollection services)
    {
        _services = services;
    }

    public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
    {
        var resolvedServiceType = serviceType;
        if (serviceType.IsInterface)
        {
            resolvedServiceType = _services.SingleOrDefault(x => x.ServiceType == serviceType)?.ImplementationType ?? serviceType;
        }
        return GetMetadata(method, contractType, resolvedServiceType);
    }
}