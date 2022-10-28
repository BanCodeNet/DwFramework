namespace DwFramework.Core;

public sealed class ExceptionWithCode : Exception
{
    public int Code { get; init; }

    public ExceptionWithCode(int code, string message) : base(message)
    {
        Code = code;
    }
}