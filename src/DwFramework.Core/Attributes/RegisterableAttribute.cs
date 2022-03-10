namespace DwFramework.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class RegisterableAttribute : Attribute
{
    public Type InterfaceType { get; set; }
    public Lifetime Lifetime { get; set; } = Lifetime.InstancePerDependency;
    public bool IsAutoActivate { get; set; } = false;
}