using Autofac;
using Castle.DynamicProxy;
using NLog;

namespace DwFramework.Core.Aop;

public static class InterceptorExtension
{
    /// <summary>
    /// 配置日志拦截器
    /// </summary>
    /// <param name="host"></param>
    /// <param name="LoggerName"></param>
    /// <param name="Level"></param>
    /// <param name="invocationHandler"></param>
    /// <returns></returns>
    public static ServiceHost ConfigureLoggerInterceptor(this ServiceHost host, Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler)
        => host.ConfigureContainer(builder => builder.Register(context => new LoggerInterceptor(invocationHandler)));
}