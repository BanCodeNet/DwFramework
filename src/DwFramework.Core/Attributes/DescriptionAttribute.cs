namespace DwFramework.Core;

public sealed class DescriptionAttribute : Attribute
{
    public string Description { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="description"></param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}