﻿using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Configuration;
using System.Reflection;

namespace DwFramework.Web;

public sealed class RpcServiceBinder : ServiceBinder
{
    private readonly IServiceCollection services;

    public RpcServiceBinder(IServiceCollection services)
    {
        this.services = services;
    }

    public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
    {
        var resolvedServiceType = serviceType;
        if (serviceType.IsInterface)
            resolvedServiceType = services.SingleOrDefault(x => x.ServiceType == serviceType)?.ImplementationType ?? serviceType;

        return base.GetMetadata(method, contractType, resolvedServiceType);
    }
}