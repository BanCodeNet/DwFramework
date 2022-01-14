using System.Collections.Concurrent;

namespace DwFramework.Core.Mapper;

public static class Mapper
{
    private static readonly ConcurrentDictionary<Type, object> _typeCache = new ConcurrentDictionary<Type, object>();
}