using Autofac;
using Autofac.Extras.DynamicProxy;
using DwFramework;
using DwFramework.Extensions.Cache;

namespace CoreExample;

class Program
{
    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args: args);
        host.UserNLog("NLog.config");
        host.UseMemoryCache();
        host.ConfigureContainer(builder =>
        {
            builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
            builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
        });
        host.UseLoggerInterceptor(invocation => (
            $"{invocation.TargetType.Name}InvokeLog",
            LogLevel.Debug,
            "\n========================================\n"
            + $"Method:\t{invocation.Method}\n"
            + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
            + $"Return:\t{invocation.ReturnValue}\n"
            + "========================================"
        ));
        host.RegisterFromAssemblies();
        host.OnHostStarted += async provider =>
        {
            var x = provider.GetService<ICache>();
            x.Add("1:1", new { A = 1, B = "1" });
            x.Add("1:2", new { A = 2, B = "2" });
            x.Add("2:1", new { A = 3, B = "3" });
            var keys = x.GetKeysWhere("default", item =>
            {
                if (item is not string) return false;
                return ((string)item).StartsWith("1:");
            });
            foreach (var key in keys) Console.WriteLine(x.Get(key));
            // foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
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
[Intercept(typeof(LoggingInterceptor))]
public class A : I
{
    public A() { }

    public int Do(int a, int b)
    {
        return a + b;
    }
}

// 定义实现
[Intercept(typeof(LoggingInterceptor))]
public class B : I
{
    public B() { }

    public int Do(int a, int b)
    {
        return a * b;
    }
}

