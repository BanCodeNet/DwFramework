using SqlSugar;
using System.Text.Json.Serialization;

namespace DwFramework.SqlSugar;

public readonly record struct Config
{
    public Dictionary<string, DbConnectionConfig> ConnectionConfigs { get; init; }
}

public readonly record struct DbConnectionConfig
{
    public string ConnectionString { get; init; }
    [JsonConverter(typeof(DbType))]
    public DbType DbType { get; init; }
    public SlaveDbConnectionConfig[] SlaveConnections { get; init; }
    public bool UseMemoryCache { get; init; } = false;
}

public readonly record struct SlaveDbConnectionConfig
{
    public string ConnectionString { get; init; }
    public int HitRate { get; init; }
}