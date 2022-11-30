using Autofac;
using Castle.DynamicProxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DwFramework;

public static class LoggingExtension
{
    /// <summary>
    /// 使用NLog服务
    /// </summary>
    /// <param name="host"></param>
    /// <param name="minimumLevel"></param>
    /// <returns></returns>
    public static ServiceHost UserNLog(this ServiceHost host, LogLevel minimumLevel = LogLevel.Trace)
    {
        return host.ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minimumLevel);
            builder.AddNLog();
        });
    }

    /// <summary>
    /// 使用NLog服务
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configuration"></param>
    /// <param name="minimumLevel"></param>
    /// <returns></returns>
    public static ServiceHost UserNLog(this ServiceHost host, IConfiguration configuration, LogLevel minimumLevel = LogLevel.Trace)
    {
        return host.ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minimumLevel);
            builder.AddNLog(configuration);
        });
    }

    /// <summary>
    /// 使用NLog服务
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configPath"></param>
    /// <param name="minimumLevel"></param>
    /// <returns></returns>
    public static ServiceHost UserNLog(this ServiceHost host, string configPath, LogLevel minimumLevel = LogLevel.Trace)
    {
        return host.ConfigureLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(minimumLevel);
            builder.AddNLog(configPath);
        });
    }

    /// <summary>
    /// 使用日志拦截器
    /// </summary>
    /// <param name="host"></param>
    /// <param name="invocationHandler"></param>
    /// <returns></returns>
    public static ServiceHost UseLoggerInterceptor(
        this ServiceHost host,
        Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler
    )
    {
        return host.ConfigureContainer(builder =>
            builder.Register(context => new LoggingInterceptor(context.Resolve<ILoggerFactory>(), invocationHandler))
        );
    }
}