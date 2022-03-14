using DwFramework.Core;

class Program
{
    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args: args);
        host.ConfigureLogging(builder => builder.UserNLog("NLog.config"));
        host.OnHostStarted += provider =>
        {
            var auther = provider.GetConfiguration().ParseConfiguration<string>("Auther");
            Console.WriteLine($"Hello {auther}!");
        };
        await host.RunAsync();
    }
}