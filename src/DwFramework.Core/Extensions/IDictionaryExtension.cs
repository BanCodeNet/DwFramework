using System.Collections.Generic;
using System.Linq;

namespace DwFramework.Core;

public static class IDictionaryExtension
{
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
