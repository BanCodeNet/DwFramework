using Autofac;
using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Reflection;

namespace DwFramework.Core;

public sealed class ServiceHost
{
    private readonly Dictionary<Type, Delegate> _options = new();
    private readonly IHostBuilder _hostBuilder;
    private static IHost? _host;

    public static string[]? Args { get; private set; }
    public static string? EnvironmentType { get; private set; }
    public static IServiceProvider? ServiceProvider => _host?.Services;
    public event Action<IServiceProvider> OnHostStarted;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="args"></param>
    public ServiceHost(params string[] args)
    {
        Args = args;
        _hostBuilder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory());
        OnHostStarted += _ => { };
    }

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ServiceHost AddOptions<T>(Action<T> action) where T : OptionsBase
    {
        _options[typeof(T)] = action;
        return this;
    }

    /// <summary>
    /// 配置主机构造器
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public ServiceHost ConfigureHostBuilder(Action<IHostBuilder> configure)
    {
        configure(_hostBuilder);
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
    /// 添加配置
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public ServiceHost AddConfiguration(IConfiguration configuration)
    {
        _hostBuilder.ConfigureHostConfiguration(builder => builder.AddConfiguration(configuration));
        return this;
    }

    /// <summary>
    /// 添加Json配置
    /// </summary>
    /// <param name="path"></param>
    /// <param name="optional"></param>
    /// <param name="reloadOnChange"></param>
    /// <returns></returns>
    public ServiceHost AddJsonConfiguration(string path, bool optional = false, bool reloadOnChange = false)
        => AddConfiguration(new ConfigurationBuilder().AddJsonFile(path, optional, reloadOnChange).Build());

    /// <summary>
    /// 添加Json配置
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public ServiceHost AddJsonConfiguration(Stream stream)
        => AddConfiguration(new ConfigurationBuilder().AddJsonStream(stream).Build());

    /// <summary>
    /// 添加Xml配置
    /// </summary>
    /// <param name="path"></param>
    /// <param name="optional"></param>
    /// <param name="reloadOnChange"></param>
    /// <returns></returns>         
    public ServiceHost AddXmlConfiguration(string path, bool optional = false, bool reloadOnChange = false)
        => AddConfiguration(new ConfigurationBuilder().AddXmlFile(path, optional, reloadOnChange).Build());

    /// <summary>
    /// 添加Xml配置
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>    
    public ServiceHost AddXmlConfiguration(Stream stream)
        => AddConfiguration(new ConfigurationBuilder().AddXmlStream(stream).Build());

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
            var attr = item.GetCustomAttribute<RegisterableAttribute>();
            if (attr == null) continue;
            if (!expression.Compile()(item)) continue;
            _hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                var registration = builder.RegisterType(item);
                registration = attr.InterfaceType != null ? registration.As(attr.InterfaceType) : registration.AsSelf();
                registration = attr.Lifetime switch
                {
                    Lifetime.Singleton => registration.SingleInstance(),
                    Lifetime.InstancePerLifetimeScope => registration.InstancePerLifetimeScope(),
                    _ => registration
                };
                if (attr.IsAutoActivate) registration.AutoActivate();
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
        if (assemblies == null) return;
        foreach (var item in assemblies)
            Assembly.Load(item);
        foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            RegisterFromAssembly(item, expression);
    }

    /// <summary>
    /// 运行服务
    /// </summary>
    /// <returns></returns>
    public async Task RunAsync()
    {
        async Task RunHostAsync()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE")))
                EnvironmentType = Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE");
            _hostBuilder.UseEnvironment(EnvironmentType ??= "Development");
            _hostBuilder.UseConsoleLifetime();
            _host = _hostBuilder.Build();
            OnHostStarted?.Invoke(ServiceProvider ?? throw new ExceptionBase(ExceptionType.Internal, 0, "ServiceProvider缺失"));
            await _host.RunAsync();
        }

        if (_options.Count <= 0) await RunHostAsync();
        else
        {
            await Parser.Default.ParseArguments(Args, _options.Keys.ToArray())
                .WithNotParsed(errors =>
                {

                })
                .WithParsedAsync(async options =>
                {
                    foreach (var item in _options)
                    {
                        if (options.GetType() != item.Key) continue;
                        item.Value.DynamicInvoke(options);
                    }
                    EnvironmentType = ((OptionsBase)options).Environment;
                    await RunHostAsync();
                });
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync() => await _host.WaitForShutdownAsync();

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IConfiguration? GetConfiguration(string? path = null)
        => ServiceProvider?.GetConfiguration(path);

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? ParseConfiguration<T>(string? path = null)
        => (ServiceProvider ?? throw new ExceptionBase(ExceptionType.Internal, 0, "ServiceProvider缺失")).ParseConfiguration<T>(path);
}