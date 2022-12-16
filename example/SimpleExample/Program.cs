using Autofac;
using DwFramework;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleExample;

class Program
{
    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args);
        host.ConfigureContainer(builder =>
        {
            builder.RegisterType<Test1>().As<ITest>();
            builder.RegisterType<Test2>().As<ITest>();
        });
        host.OnHostStarted += p =>
        {
            foreach (var t in p.GetServices<ITest>())
            {
                Console.WriteLine(t.Do(2, 3));
            }
            return Task.CompletedTask;
        };
        await host.RunAsync();
    }
}