using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DwFramework;

public static class ServiceHostExtension
{
    /// <summary>
    /// 添加配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static ServiceHost AddConfiguration(this ServiceHost host, IConfiguration configuration)
    {
        host.ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration));
        return host;
    }

    /// <summary>
    /// 添加Json配置
    /// </summary>
    /// <param name="host"></param>
    /// <param name="path"></param>
    /// <param name="optional"></param>
    /// <param name="reloadOnChange"></param>
    /// <returns></returns>
    public static ServiceHost AddJsonConfiguration(
        this ServiceHost host,
        string path,
        bool optional = false,
        bool reloadOnChange = false
    )
    {
        return host.AddConfiguration(new ConfigurationBuilder()
            .AddJsonFile(path, optional, reloadOnChange)
            .Build());
    }

    /// <summary>
    /// 添加Json配置
    /// </summary>
    /// <param name="host"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static ServiceHost AddJsonConfiguration(this ServiceHost host, Stream stream)
    {
        return host.AddConfiguration(new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build());
    }

    /// <summary>
    /// 添加Xml配置
    /// </summary>
    /// <param name="host"></param>
    /// <param name="path"></param>
    /// <param name="optional"></param>
    /// <param name="reloadOnChange"></param>
    /// <returns></returns>     
    public static ServiceHost AddXmlConfiguration(
        this ServiceHost host,
        string path,
        bool optional = false,
        bool reloadOnChange = false
    )
    {
        return host.AddConfiguration(new ConfigurationBuilder()
            .AddXmlFile(path, optional, reloadOnChange)
            .Build());
    }

    /// <summary>
    /// 添加Xml配置
    /// </summary>
    /// <param name="host"></param>
    /// <param name="stream"></param>
    /// <returns></returns>  
    public static ServiceHost AddXmlConfiguration(this ServiceHost host, Stream stream)
    {
        return host.AddConfiguration(new ConfigurationBuilder()
            .AddXmlStream(stream)
            .Build());
    }
}