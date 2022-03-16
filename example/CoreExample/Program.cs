using Autofac;
using Autofac.Extras.DynamicProxy;
using DwFramework.Core;
using DwFramework.Core.Time;
using DwFramework.Core.Encrypt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CommandLine;

namespace CoreExample;

class Program
{
    [Verb("commit", HelpText = "Record changes to the repository.")]
    class CommitOptions : OptionsBase
    {
        [Option('a', "alpha", Required = true)]
        public string Alpha { get; set; }
        [Option('X', "xop")]
        public bool Xop { get; set; }
    }

    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args: args);
        // host.AddOptions<CommitOptions>(opts =>
        // {
        //     Console.WriteLine(opts.Alpha);
        // });
        // host.AddCommand("X", (int a, int c, string b, bool d, FileAccess f) =>
        // {
        //     Console.WriteLine($"a = {a},b = {b},c = {c},d = {d},f = {f}");
        // }, new Option[]{
        //     new Option<int>(new []{"-a","--aa"},()=>-1,"AA"),
        //     new Option<string>(new []{"-b","--bb"},"BB"),
        //     new Option<int>(new []{"-c","--cc"},"CC"),
        //     new Option<bool>("-d"),
        //     new Option<FileAccess>("-f",()=>FileAccess.Write)
        // }, "测试1");
        // host.AddCommand("Y", (int a, int c, string b, bool d, FileAccess f) =>
        // {
        //     Console.WriteLine($"a = {a},b = {b},c = {c},d = {d},f = {f}");
        // }, new Option[]{
        //     new Option<int>(new []{"-a","--aa"},()=>-1,"AA"),
        //     new Option<string>(new []{"-b","--bb"},"BB"),
        //     new Option<int>(new []{"-c","--cc"},"CC"),
        //     new Option<bool>("-d"),
        //     new Option<FileAccess>("-f")
        // }, "测试2");
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

