using DwFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UseNLog;

class Program
{
    static async Task Main(params string[] args)
    {
        var host = new ServiceHost(args: args);
        host.UserNLog("config.xml");
        host.OnInitialized += p =>
        {
            var factory = p.GetService<ILoggerFactory>();
            var logger = factory.CreateLogger("test");
            logger.LogDebug("log output");
            logger.LogCritical("critical output");
            logger.LogError("error output");
            logger.LogInformation("information output");
            logger.LogTrace("trace output");
            logger.LogWarning("warning output");
            return Task.CompletedTask;
        };
        await host.RunAsync();
    }
}