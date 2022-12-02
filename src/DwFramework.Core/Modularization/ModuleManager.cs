using System.Reflection;

namespace DwFramework;

public static class ModuleManager
{
    private sealed class ModuleInfo
    {
        public string Path { get; set; }
        public object Instance { get; set; }
    }

    private static Dictionary<Type, ModuleInfo> _loadedModules { get; } = new();

    /// <summary>
    /// 加载模块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="modulePath"></param>
    /// <returns></returns>
    public static T LoadModule<T>(string modulePath)
    {
        var moduleType = typeof(T);
        if (_loadedModules.ContainsKey(moduleType))
        {
            var instance = _loadedModules[moduleType].Instance;
            if (instance is null) _loadedModules.Remove(moduleType);
            else return (T)instance;
        }
        if (File.Exists(modulePath))
        {
            var loadContext = new ModuleLoadContext(modulePath);
            var assembly = loadContext.LoadFromAssemblyName(
                new AssemblyName(Path.GetFileNameWithoutExtension(modulePath))
            );
            foreach (Type type in assembly.GetTypes())
            {
                if (moduleType.IsAssignableFrom(type) && Activator.CreateInstance(type) is T instance)
                {
                    _loadedModules[moduleType] = new ModuleInfo()
                    {
                        Path = modulePath,
                        Instance = instance
                    };
                    return instance;
                }
            }
            return default;
        }
        else throw new ExceptionWithCode(ErrorCode.ParameterError, $"the path is not existed: {modulePath}");
    }

    /// <summary>
    /// 卸载模块
    /// </summary>
    public static void UnloadModule<T>()
    {
        var moduleType = typeof(T);
        if (!_loadedModules.ContainsKey(moduleType)) return;
        var path = _loadedModules[moduleType].Path;
        if (!string.IsNullOrEmpty(path)) new ModuleLoadContext(path).Unload();
        _loadedModules.Remove(moduleType);
    }

    /// <summary>
    /// 卸载所有模块
    /// </summary>
    public static void UnloadAllModules()
    {
        foreach (var item in _loadedModules)
        {
            var path = item.Value.Path;
            if (!string.IsNullOrEmpty(path)) new ModuleLoadContext(path).Unload();
            _loadedModules.Remove(item.Key);
        }
    }
}