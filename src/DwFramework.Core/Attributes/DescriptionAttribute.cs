namespace DwFramework.Core;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public sealed class DescriptionAttribute : Attribute
{
    public string Description { get; init; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="description"></param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}