namespace DwFramework.Core;

public sealed class TypeNotMatchException : BaseException
{
    public readonly Type SourceType;
    public readonly Type TargetType;

    public TypeNotMatchException(Type sourceType, Type targetType) : base(402, $"{sourceType}与{targetType}类型不匹配")
    {
        SourceType = sourceType;
        TargetType = targetType;
    }
}