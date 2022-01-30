namespace DwFramework.Core;

public enum ExceptionType
{
    [Description("未知异常")]
    Unknown = 10000,
    [Description("类型异常")]
    Type = 20000,
    [Description("参数异常")]
    Parameter = 30000,
    [Description("内部异常")]
    Internal = 40000
}