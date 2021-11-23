using Autofac;
using Castle.DynamicProxy;
using NLog;

namespace DwFramework.Core.Aop;

public static class InterceptorExtension
{
    /// <summary>
    /// 注册日志拦截器
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="LoggerName"></param>
    /// <param name="Level"></param>
    /// <param name="invocationHandler"></param>
    /// <returns></returns>
    public static ContainerBuilder RegisterLoggerInterceptor(this ContainerBuilder builder, Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler)
    {
        builder.Register(context => new LoggerInterceptor(invocationHandler));
        return builder;
    }
}