using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DwFramework;

public static class LoggingExtension
{
    /// <summary>
    /// 注册NLog服务
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ILoggingBuilder UserNLog(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Trace);
        builder.AddNLog();
        return builder;
    }

    /// <summary>
    /// 注册NLog服务
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ILoggingBuilder UserNLog(
        this ILoggingBuilder builder,
        IConfiguration configuration,
        LogLevel minimumLevel = LogLevel.Trace)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(minimumLevel);
        builder.AddNLog(configuration);
        return builder;
    }

    /// <summary>
    /// 注册NLog服务
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configPath"></param>
    /// <returns></returns>
    public static ILoggingBuilder UserNLog(
        this ILoggingBuilder builder,
        string configPath,
        LogLevel minimumLevel = LogLevel.Trace
    )
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(minimumLevel);
        builder.AddNLog(configPath);
        return builder;
    }

    /// <summary>
    /// 获取Logger工厂
    /// </summary>
    /// <param name="provider"></param>
    /// <typeparam name="ILoggerFactory"></typeparam>
    /// <returns></returns>
    public static ILoggerFactory GetLoggerFactory(this IServiceProvider provider)
    {
        return provider.GetService<ILoggerFactory>();
    }

    /// <summary>
    /// 获取Logger
    /// </summary>
    /// <param name="provider"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILogger<T> GetLogger<T>(this IServiceProvider provider)
    {
        return provider.GetService<ILogger<T>>();
    }

    /// <summary>
    /// 获取Logger
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static ILogger GetLogger(this IServiceProvider provider, Type type)
    {
        return provider.GetLoggerFactory()?.CreateLogger(type);
    }

    /// <summary>
    /// 获取Logger
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ILogger GetLogger(this IServiceProvider provider, string name)
    {
        return provider.GetLoggerFactory()?.CreateLogger(name);
    }
}