using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace DwFramework.Core.Aop;

public sealed class LoggerInterceptor : IInterceptor
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> _invocationHandler;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="LoggerName"></param>
    /// <param name="Level"></param>
    /// <param name="invocationHandler"></param>
    public LoggerInterceptor(ILoggerFactory loggerFactory, Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler)
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
        if (logger == null) return;
        logger?.Log(result.Level, result.Context);
    }
}