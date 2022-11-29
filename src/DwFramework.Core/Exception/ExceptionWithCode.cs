namespace DwFramework;

public sealed class ExceptionWithCode : Exception
{
    public int Code { get; init; }

    /// <summary>
    /// 包含错误码异常
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public ExceptionWithCode(int code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// 包含错误码异常
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    /// <returns></returns>
    public ExceptionWithCode(int code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }
}