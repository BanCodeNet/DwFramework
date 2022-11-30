using Autofac;

namespace DwFramework.Extensions.Cache;

public static class CacheExtension
{
    /// <summary>
    /// 使用MemoryCache
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    public static ServiceHost UseMemoryCache(this ServiceHost host)
    {
        return host.ConfigureContainer(builder =>
            builder.RegisterType<MemoryCache>()
            .As<ICache>()
            .SingleInstance()
        );
    }
}