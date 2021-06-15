﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;
using DwFramework.Core.AOP;
using ilib;

namespace CoreExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var x = ModuleManager.LoadModule<IClass1>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib/lib.dll"));
            ModuleManager.UnloadModule<IClass1>();
            // var y = ModuleManager.LoadModule<IClass1>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib/lib.dll"));
            await Task.Delay(1000);
            Console.WriteLine(x.Do(5, 8));
            // Console.WriteLine(y.Do(5, 2));

            // var host = new ServiceHost();
            // host.ConfigureLogging(builder => builder.UserNLog());
            // host.ConfigureContainer(builder =>
            // {
            //     builder.Register(c => new LoggerInterceptor(invocation => (
            //         $"{invocation.TargetType.Name}InvokeLog",
            //         LogLevel.Debug,
            //         "\n========================================\n"
            //         + $"Method:\t{invocation.Method}\n"
            //         + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
            //         + $"Return:\t{invocation.ReturnValue}\n"
            //         + "========================================"
            //     )));
            //     builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
            //     builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
            // });
            // host.OnHostStarted += provider =>
            // {
            //     foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
            // };
            // await host.RunAsync();
        }
    }

    // 定义接口
    public interface I
    {
        int Do(int a, int b);
    }

    // 定义实现
    public class A : I
    {
        public A() { }

        public int Do(int a, int b)
        {
            return a + b;
        }
    }

    // 定义实现
    public class B : I
    {
        public B() { }

        public int Do(int a, int b)
        {
            return a * b;
        }
    }
}

