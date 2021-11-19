namespace DwFramework.Core;

public abstract class BaseException : Exception
{
    public int Code { get; init; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public BaseException(int code, string message = null, Exception innerException = null) : base(message, innerException)
    {
        Code = code;
    }
}