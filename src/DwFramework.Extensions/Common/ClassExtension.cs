namespace DwFramework.Extensions;

public static class ClassExtension
{
    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? To<T>(this object? obj)
    {
        return obj is null ? default : (T)obj;
    }
}