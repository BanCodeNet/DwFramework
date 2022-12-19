using Microsoft.Extensions.Configuration;

namespace DwFramework;

public static class ConfigurationExtension
{
    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IConfiguration GetConfiguration(this IConfiguration configuration, string path)
    {
        return string.IsNullOrEmpty(path) ? configuration : configuration.GetSection(path);
    }

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseConfiguration<T>(this IConfiguration configuration, string path)
    {
        return configuration.GetConfiguration(path).Get<T>();
    }
}