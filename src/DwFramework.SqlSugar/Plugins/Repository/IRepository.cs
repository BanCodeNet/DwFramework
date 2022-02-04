using SqlSugar;
using System.Linq.Expressions;
using System.Reflection;

namespace DwFramework.SqlSugar.Repository;

public interface IRepository<T> where T : class, new()
{
    public SqlSugarClient CreateConnection(InitKeyType initKeyType = InitKeyType.Attribute, Action<Type, EntityInfo> entityNameService = null, Action<PropertyInfo, EntityColumnInfo> entityService = null);
    public ISugarQueryable<T> Select(SqlSugarClient conn, Expression<Func<T, bool>> expression = null);
    public Task<List<T>> InsertOrUpdateAsync(SqlSugarClient conn, List<T> objs, Expression<Func<T, object>> identity = null);
    public Task<bool> DeleteAsync(SqlSugarClient conn, Expression<Func<T, bool>> expression = null);
}