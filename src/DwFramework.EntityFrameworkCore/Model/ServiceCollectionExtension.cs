using Microsoft.Extensions.DependencyInjection;

namespace DwFramework.EntityFrameworkCore;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// 添加DbContextFactory
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDbContextFactory(this IServiceCollection services)
    {
        services.AddScoped<DbContextFactory>();
        return services;
    }
}