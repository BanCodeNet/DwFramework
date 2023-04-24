namespace DwFramework;

/// <summary>
/// 可注册类
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RegisterableAttribute : Attribute
{
    public Type? InterfaceType { get; init; } = null;
    public Lifetime Lifetime { get; init; } = Lifetime.InstancePerDependency;
    public bool IsAutoActivate { get; init; } = false;
}