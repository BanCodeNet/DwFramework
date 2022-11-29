using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace DwFramework;

public sealed class LoggingInterceptor : IInterceptor
{
    private ILoggerFactory _loggerFactory { get; init; }
    private Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> _invocationHandler { get; init; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="LoggerName"></param>
    /// <param name="Level"></param>
    /// <param name="invocationHandler"></param>
    /// <returns></returns>
    public LoggingInterceptor(ILoggerFactory loggerFactory, Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler)
    {
        _loggerFactory = loggerFactory;
        _invocationHandler = invocationHandler;
    }

    /// <summary>
    /// 拦截调用
    /// </summary>
    /// <param name="invocation"></param>
    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();
        var result = _invocationHandler(invocation);
        var logger = _loggerFactory.CreateLogger(result.LoggerName);
        logger?.Log(result.Level, result.Context);
    }
}