using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using NLog;
using DwFramework.Core;
using DwFramework.Core.Aop;

namespace CoreExample;

class Program
{
    class XX
    {
        public string Sex { get; set; }
    }

    class X
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public XX XX { get; set; }
    }

    static async Task Main(string[] args)
    {
        var x = new X() { Id = 1, Name = "XX", XX = new XX() { Sex = "男" } };
        var y = x.DeepClone();
        x.XX.Sex = "YY";
        Console.WriteLine("X ===> " + x.ToJson());
        Console.WriteLine("Y ===> " + y.ToJson());

        // var host = new ServiceHost();
        // host.ConfigureLogging(builder => builder.UserNLog("NLog.config"));
        // host.ConfigureContainer(builder =>
        // {
        //     builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
        //     builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
        // });
        // host.ConfigureLoggerInterceptor(invocation => (
        //     $"{invocation.TargetType.Name}InvokeLog",
        //     LogLevel.Debug,
        //     "\n========================================\n"
        //     + $"Method:\t{invocation.Method}\n"
        //     + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
        //     + $"Return:\t{invocation.ReturnValue}\n"
        //     + "========================================"
        // ));
        // host.OnHostStarted += provider =>
        // {
        //     var x = ServiceHost.ParseConfiguration<string>("ConnectionString");
        //     var y = provider.GetServices<I>();
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

