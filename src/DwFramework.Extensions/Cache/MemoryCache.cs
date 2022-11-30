using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace DwFramework.Extensions.Cache;

public sealed class MemoryCache : ICache
{
    private Dictionary<string, Microsoft.Extensions.Caching.Memory.MemoryCache> _memoryCaches { get; init; } = new();
    private Dictionary<string, HashSet<object>> _keys { get; init; } = new();

    /// <summary>
    /// 构造函数
    /// </summary>
    public MemoryCache() { }

    /// <summary>
    /// 创建组
    /// </summary>
    /// <param name="groupName"></param>
    public void CreateGroup(string groupName)
    {
        if (_memoryCaches.ContainsKey(groupName)) throw new Exception("the group has already existed");
        _memoryCaches[groupName] = new(new MemoryCacheOptions());
        _keys[groupName] = new();
    }

    /// <summary>
    /// 删除组
    /// </summary>
    /// <param name="groupName"></param>
    public void DropGroup(string groupName)
    {
        if (!_memoryCaches.ContainsKey(groupName)) return;
        _memoryCaches.Remove(groupName);
        _keys.Remove(groupName);
    }

    /// <summary>
    /// 添加数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="groupName"></param>
    /// <typeparam name="T"></typeparam>
    public void Add<T>(object key, T value, string groupName = "default")
    {
        if (!_memoryCaches.ContainsKey(groupName)) CreateGroup(groupName);
        _memoryCaches[groupName].Set(key, value);
        _keys[groupName].Add(key);
    }

    /// <summary>
    /// 添加数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireAt"></param>
    /// <param name="groupName"></param>
    /// <typeparam name="T"></typeparam>
    public void Add<T>(object key, T value, DateTime expireAt, string groupName = "default")
    {
        if (!_memoryCaches.ContainsKey(groupName)) CreateGroup(groupName);
        _memoryCaches[groupName].Set(key, value, new MemoryCacheEntryOptions() { AbsoluteExpiration = expireAt });
        _keys[groupName].Add(key);
    }

    /// <summary>
    /// 添加数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expireTime"></param>
    /// <param name="isSliding"></param>
    /// <param name="groupName"></param>
    /// <typeparam name="T"></typeparam>
    public void Add<T>(object key, T value, TimeSpan expireTime, bool isSliding = false, string groupName = "default")
    {
        if (!_memoryCaches.ContainsKey(groupName)) CreateGroup(groupName);
        _memoryCaches[groupName].Set(key, value, isSliding
            ? new MemoryCacheEntryOptions()
            {
                SlidingExpiration = expireTime
            }
            : new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expireTime
            });
        _keys[groupName].Add(key);
    }

    /// <summary>
    /// 获取数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public object Get(object key, string groupName = "default")
    {
        if (!_memoryCaches.ContainsKey(groupName)) throw new Exception("the group has not existed");
        return _memoryCaches[groupName].Get(key);
    }

    /// <summary>
    /// 获取数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName"></param>
    /// <typeparam name="T"></typeparam>
    public T Get<T>(object key, string groupName = "default") where T : class
    {
        if (!_memoryCaches.ContainsKey(groupName)) throw new Exception("the group has not existed");
        return _memoryCaches[groupName].Get<T>(key);
    }

    /// <summary>
    /// 删除数据（对象）
    /// </summary>
    /// <param name="key"></param>
    /// <param name="groupName"></param>
    public void Delete(object key, string groupName = "default")
    {
        if (!_memoryCaches.ContainsKey(groupName)) throw new Exception("the group has not existed");
        if (!_keys[groupName].Remove(key)) return;
        _memoryCaches[groupName].Remove(key);
    }

    /// <summary>
    /// 获取所有Key
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    public Dictionary<string, object[]> GetAllKeys(string groupName = null)
    {
        return _keys.Where(item => groupName is null ? true : item.Key == groupName)
            .ToDictionary(item => item.Key, item => item.Value.ToArray());
    }

    /// <summary>
    /// 条件查询Key
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public object[] GetKeysWhere(string groupName, Func<object, bool> conditions)
    {
        if (!_memoryCaches.ContainsKey(groupName)) throw new Exception("the group has not existed");
        return _keys[groupName].Where(item => conditions.Invoke(item)).ToArray();
    }
}