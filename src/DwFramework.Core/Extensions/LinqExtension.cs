namespace DwFramework.Core;

public static class LinqExtension
{
    /// <summary>
    /// 按字段去重
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
    {
        var hash = new HashSet<TKey>();
        return source.Where(item => hash.Add(selector(item)));
    }

    /// <summary>
    /// 转HashSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static HashSet<TResult> ToHashSet<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
    {
        var set = new HashSet<TResult>();
        set.UnionWith(source.Select(selector));
        return set;
    }

    /// <summary>
    /// 条件删除
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="selector"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static void Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<KeyValuePair<TKey, TValue>, bool> selector)
    {
        var keys = dictionary.Where(item => selector(item)).Select(item => item.Key);
        foreach (var key in keys) dictionary.Remove(key);
    }
}