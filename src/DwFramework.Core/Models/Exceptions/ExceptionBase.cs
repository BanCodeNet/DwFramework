namespace DwFramework.Core;

public class ExceptionBase : Exception
{
    public int Code { get; init; }
    public override string Message { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    /// <returns></returns>
    public ExceptionBase(ExceptionType type, int code = 0, string? message = null, Exception? innerException = null) : base(message, innerException)
    {
        Code = (int)type + code;
        Message = $"{type.GetDescription()}{(string.IsNullOrEmpty(message) ? "" : $": {message}")}";
    }
}