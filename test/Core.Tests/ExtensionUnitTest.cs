using DwFramework.Core;
using DwFramework.Core.Generator;
using Xunit;

namespace Core.Tests;

public class ExtensionUnitTest
{
    public ExtensionUnitTest()
    {
        var host = new ServiceHost();
        host.ConfigureSnowflakeGenerator(1, DateTime.Parse("1970.01.01"));
        _ = host.RunAsync();
    }

    [Fact]
    public void GenerateSnowflakeId()
    {
        var generator = ServiceHost.ServiceProvider.GetSnowflakeGenerator();
        var id = generator.GenerateId();
        Console.WriteLine(id);
    }
}