﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using DwFramework.Core;
using DwFramework.Core.Generator;
using DwFramework.Core.Time;
using DwFramework.Core.Encrypt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;

namespace CoreExample;

class Program
{
    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args: args);
        host.ConfigureSnowflakeGenerator(1, DateTime.Parse("2022.01.01"), false);
        host.ConfigureLogging(builder => builder.UserNLog("NLog.config"));
        host.ConfigureContainer(builder =>
        {
            builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
            builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
        });
        host.ConfigureLoggerInterceptor(invocation => (
            $"{invocation.TargetType.Name}InvokeLog",
            LogLevel.Debug,
            "\n========================================\n"
            + $"Method:\t{invocation.Method}\n"
            + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
            + $"Return:\t{invocation.ReturnValue}\n"
            + "========================================"
        ));
        host.RegisterFromAssemblies();
        host.OnHostStarted += provider =>
        {
            var g = provider.GetService<SnowflakeGenerator>().GenerateId();
            var x = ServiceHost.ParseConfiguration<string>("ConnectionString");
            var y = provider.GetServices<I>();
            foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
        };
        await host.RunAsync();
    }
}

// 定义接口
public interface I
{
    int Do(int a, int b);
}

// 定义实现
[Intercept(typeof(LoggerInterceptor))]
public class A : I
{
    public A() { }

    public int Do(int a, int b)
    {
        return a + b;
    }
}

// 定义实现
[Intercept(typeof(LoggerInterceptor))]
public class B : I
{
    public B() { }

    public int Do(int a, int b)
    {
        return a * b;
    }
}

