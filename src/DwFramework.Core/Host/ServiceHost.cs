using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;

namespace DwFramework;

public sealed class ServiceHost
{
    private IHostBuilder _hostBuilder { get; init; }
    private static IHost? _host { get; set; }

    public event Func<IServiceProvider, Task> OnInitialized = _ => Task.CompletedTask;
    public event Func<IServiceProvider, Task> OnStopping = _ => Task.CompletedTask;
    public event Func<Task> OnStopped = () => Task.CompletedTask;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="args"></param>
    public ServiceHost(params string[] args)
    {
        _hostBuilder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory());
        _hostBuilder.UseEnvironment(Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE") ?? "Development");
        _hostBuilder.UseConsoleLifetime();
    }

    /// <summary>
    /// 配置主机构造器
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureHostBuilder(Action<IHostBuilder> configure)
    {
        configure?.Invoke(_hostBuilder);
        return this;
    }

    /// <summary>
    /// 配置主机
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureHostConfiguration(Action<IConfigurationBuilder> configure)
    {
        _hostBuilder.ConfigureHostConfiguration(configure);
        return this;
    }

    /// <summary>
    /// 配置应用
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureAppConfiguration(Action<IConfigurationBuilder> configure)
    {
        _hostBuilder.ConfigureAppConfiguration(configure);
        return this;
    }

    /// <summary>
    /// 配置应用
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure)
    {
        _hostBuilder.ConfigureAppConfiguration(configure);
        return this;
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureServices(Action<IServiceCollection> configure)
    {
        _hostBuilder.ConfigureServices(configure);
        return this;
    }

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
    {
        _hostBuilder.ConfigureServices(configure);
        return this;
    }

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureLogging(Action<ILoggingBuilder> configure)
    {
        _hostBuilder.ConfigureLogging(configure);
        return this;
    }

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureLogging(Action<HostBuilderContext, ILoggingBuilder> configure)
    {
        _hostBuilder.ConfigureLogging(configure);
        return this;
    }

    /// <summary>
    /// 配置容器
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureContainer(Action<ContainerBuilder> configure)
    {
        _hostBuilder.ConfigureContainer(configure);
        return this;
    }

    /// <summary>
    /// 配置容器
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureContainer(Action<HostBuilderContext, ContainerBuilder> configure)
    {
        _hostBuilder.ConfigureContainer(configure);
        return this;
    }

    /// <summary>
    /// 配置主机选项
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureHostOptions(Action<HostOptions> configure)
    {
        _hostBuilder.ConfigureHostOptions(configure);
        return this;
    }

    /// <summary>
    /// 配置主机选项
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureHostOptions(Action<HostBuilderContext, HostOptions> configure)
    {
        _hostBuilder.ConfigureHostOptions(configure);
        return this;
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="expression"></param>
    public void RegisterFromAssembly(Assembly assembly, Expression<Func<Type, bool>>? expression = null)
    {
        expression ??= _ => true;
        foreach (var item in assembly.GetTypes())
        {
            var attribute = item.GetCustomAttribute<RegisterableAttribute>();
            if (attribute is null) continue;
            if (!expression.Compile()(item)) continue;
            _hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                var registration = builder.RegisterType(item);
                registration = attribute.InterfaceType is not null
                    ? registration.As(attribute.InterfaceType)
                    : registration.AsSelf();
                registration = attribute.Lifetime switch
                {
                    Lifetime.Singleton => registration.SingleInstance(),
                    Lifetime.InstancePerLifetimeScope => registration.InstancePerLifetimeScope(),
                    _ => registration
                };
                if (attribute.IsAutoActivate) registration.AutoActivate();
            });
        };
    }

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <param name="expression"></param>
    public void RegisterFromAssemblies(Expression<Func<Type, bool>>? expression = null)
    {
        var assemblies = Assembly.GetEntryAssembly()?.GetReferencedAssemblies();
        if (assemblies is null) return;
        foreach (var item in assemblies) Assembly.Load(item);
        foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            RegisterFromAssembly(item, expression);
    }

    /// <summary>
    /// 运行服务
    /// </summary>
    /// <returns></returns>
    public async Task RunAsync()
    {
        _host = _hostBuilder.Build();
        await OnInitialized.Invoke(_host.Services);
        await _host.RunAsync();
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        if (_host is null) return;
        await OnStopping.Invoke(_host.Services);
        await _host.WaitForShutdownAsync();
        await OnStopped.Invoke();
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static object? GetService(Type serviceType)
    {
        if (_host is null) return null;
        return _host.Services.GetService(serviceType);
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetService<T>()
    {
        if (_host is null) return default(T);
        return _host.Services.GetService<T>();
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    public static IEnumerable<object?> GetServices(Type serviceType)
    {
        if (_host is null) return Array.Empty<object>();
        return _host.Services.GetServices(serviceType);
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> GetServices<T>()
    {
        if (_host is null) return Array.Empty<T>();
        return _host.Services.GetServices<T>();
    }
}