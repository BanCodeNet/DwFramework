using System.Reflection;
using System.Runtime.Loader;

namespace DwFramework;

public sealed class ModuleLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver { get; init; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="modulePath"></param>
    /// <returns></returns>
    public ModuleLoadContext(string modulePath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(modulePath);
    }

    /// <summary>
    /// 加载程序集
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    protected override Assembly Load(AssemblyName assemblyName)
    {
        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath is null ? null : LoadFromAssemblyPath(assemblyPath);
    }

    /// <summary>
    /// 加载非托管程序集
    /// </summary>
    /// <param name="unmanagedDllName"></param>
    /// <returns></returns>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(libraryPath);
    }
}