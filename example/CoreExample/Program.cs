﻿using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using NLog;
using DwFramework.Core;
using DwFramework.Core.Generator;
using DwFramework.Core.Aop;

namespace CoreExample;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.AddJsonConfiguration("Config.json", reloadOnChange: true);
        host.ConfigureLogging(builder => builder.UserNLog());
        host.ConfigureContainer(builder =>
        {
            builder.RegisterLoggerInterceptor(invocation => (
                $"{invocation.TargetType.Name}InvokeLog",
                LogLevel.Debug,
                "\n========================================\n"
                + $"Method:\t{invocation.Method}\n"
                + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
                + $"Return:\t{invocation.ReturnValue}\n"
                + "========================================"
            ));
            builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
            builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
        });
        host.OnHostStarted += provider =>
        {
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

