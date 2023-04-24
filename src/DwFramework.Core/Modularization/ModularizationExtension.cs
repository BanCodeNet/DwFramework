using Autofac;

namespace DwFramework;

public static class ModularizationExtension
{
    /// <summary>
    /// 导入模块
    /// </summary>
    /// <param name="host"></param>
    /// <param name="modulePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ServiceHost ImportModule<T>(this ServiceHost host, string modulePath) where T : notnull
    {
        return host.ConfigureContainer(builder =>
            builder.Register(_ => ModuleManager.LoadModule<T>(modulePath))
        );
    }

    /// <summary>
    /// 导入模块
    /// </summary>
    /// <param name="host"></param>
    /// <param name="modulePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ServiceHost ImportSingleInstanceModule<T>(this ServiceHost host, string modulePath) where T : notnull
    {
        return host.ConfigureContainer(builder =>
            builder.Register(_ => ModuleManager.LoadModule<T>(modulePath)).SingleInstance()
        );
    }
}