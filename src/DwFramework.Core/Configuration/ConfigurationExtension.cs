using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DwFramework;

public static class ConfigurationExtension
{
    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IConfiguration GetConfiguration(this IConfiguration configuration, string path = null)
    {
        return configuration?.GetSection(path);
    }

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseConfiguration<T>(this IConfiguration configuration, string path = null)
    {
        if (configuration.GetConfiguration(path) is null)
            throw new ExceptionWithCode(ErrorCode.InternalError, $"{path} can not get value");
        return configuration.Get<T>();
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IConfiguration GetConfiguration(this IServiceProvider provider, string path = null)
    {
        return provider.GetService<IConfiguration>()?.GetConfiguration(path);
    }

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseConfiguration<T>(this IServiceProvider provider, string path = null)
    {
        return provider.GetConfiguration(path).ParseConfiguration<T>();
    }
}