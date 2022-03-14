﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Linq.Expressions;
using System.Reflection;

namespace DwFramework.Core;

public sealed class ServiceHost
{
    public event Action<IServiceProvider> OnHostStarted;

    private readonly RootCommand _rootCommand = new();
    private readonly CommandLineBuilder _commandLineBuilder;
    private readonly Option _environmentTypeOption = new Option<string>(new[] { "-e", "--env" }, () => "Development", "运行环境");
    private readonly IHostBuilder _hostBuilder;
    private static IHost _host;

    public static string[] Args { get; private set; }
    public static string EnvironmentType { get; private set; }
    public static IServiceProvider ServiceProvider => _host.Services;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="args"></param>
    public ServiceHost(params string[] args)
    {
        Args = args;
        _commandLineBuilder = new CommandLineBuilder(_rootCommand);
        _hostBuilder = Host.CreateDefaultBuilder(args).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="action"></param>
    /// <param name="options"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public ServiceHost AddCommand(Delegate action, IEnumerable<Option> options = null, string description = null)
    {
        _rootCommand.Handler = CommandHandler.Create(action);
        if (options.Count() > 0) foreach (var item in options) _rootCommand.AddOption(item);
        return this;
    }

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    /// <param name="options"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public ServiceHost AddCommand(string name, Delegate action, IEnumerable<Option> options = null, string description = null)
    {
        var command = new Command(name, description);
        command.Handler = CommandHandler.Create(action);
        if (options.Count() > 0) foreach (var item in options) command.AddOption(item);
        _rootCommand.Add(command);
        return this;
    }

    /// <summary>
    /// 添加命令
    /// </summary>
    /// <param name="middleware"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public ServiceHost AddMiddleware(InvocationMiddleware middleware, MiddlewareOrder order = MiddlewareOrder.Default)
    {
        _commandLineBuilder.AddMiddleware(middleware, order);
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
    public void RegisterFromAssembly(Assembly assembly, Expression<Func<Type, bool>> expression = null)
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
    public void RegisterFromAssemblies(Expression<Func<Type, bool>> expression = null)
    {
        foreach (var item in Assembly.GetEntryAssembly().GetReferencedAssemblies())
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
        _rootCommand.AddOption(_environmentTypeOption);
        _commandLineBuilder.AddMiddleware(async (context, next) =>
        {
            var environmentTypeOption = context.ParseResult.FindResultFor(_environmentTypeOption);
            EnvironmentType = environmentTypeOption?.GetValueOrDefault<string>();
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE")))
                EnvironmentType = Environment.GetEnvironmentVariable("ENVIRONMENT_TYPE");
            await next(context);
        });
        _commandLineBuilder.UseDefaults();
        var parser = _commandLineBuilder.Build();
        var result = await parser.InvokeAsync(Args);
        if (result != 0) return;
        _hostBuilder.UseEnvironment(EnvironmentType ??= "Development");
        _hostBuilder.UseConsoleLifetime();
        _host = _hostBuilder.Build();
        OnHostStarted?.Invoke(ServiceProvider);
        await _host.RunAsync();
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
    public static IConfiguration GetConfiguration(string path = null)
        => ServiceProvider.GetConfiguration(path);

    /// <summary>
    /// 解析配置
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseConfiguration<T>(string path = null)
        => ServiceProvider.ParseConfiguration<T>(path);
}