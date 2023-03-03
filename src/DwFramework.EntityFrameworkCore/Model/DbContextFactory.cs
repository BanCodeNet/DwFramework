using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Data.Common;

namespace DwFramework.EntityFrameworkCore;

public class DbContextFactory
{
    private IConfiguration _configuration { get; init; }
    private IServiceProvider _serviceProvider { get; init; }

    public DbContextFactory(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 获取数据库连接
    /// </summary>
    /// <returns></returns>
    public DbConnection CreateMySqlConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }

    /// <summary>
    /// 获取上下文
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetDbContext<T>() where T : DbContext
    {
        var dbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<T>();
        return dbContext;
    }

    /// <summary>
    /// 获取上下文
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="transaction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetDbContext<T>(DbConnection connection, DbTransaction transaction) where T : DbContext
    {
        var dbContext = GetDbContext<T>();
        dbContext.Database.SetDbConnection(connection);
        dbContext.Database.UseTransaction(transaction);
        return dbContext;
    }
}