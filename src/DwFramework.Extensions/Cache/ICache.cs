namespace DwFramework.Extensions.Cache;

public interface ICache
{
    public void CreateGroup(string groupName);
    public void DropGroup(string groupName);
    public void Add<T>(object key, T value, string groupName = "default");
    public void Add<T>(object key, T value, DateTime expireAt, string groupName = "default");
    public void Add<T>(object key, T value, TimeSpan expireTime, bool isSliding = false, string groupName = "default");
    public object Get(object key, string groupName = "default");
    public T? Get<T>(object key, string groupName = "default");
    public void Delete(object key, string groupName = "default");
    public Dictionary<string, object[]> GetAllKeys(string? groupName = null);
    public object[] GetKeysWhere(string groupName, Func<object, bool> conditions);
}