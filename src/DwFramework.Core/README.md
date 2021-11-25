# DwFramework.Core

```shell
PM> Install-Package DwFramework.Core
或
> dotnet add package DwFramework.Core
```

## DwFramework 核心库

### 0x1 依赖注入
```c#
public interface ITest
{
    void Do();
}

[Registerable(typeof(ITest))]
public class Test1 : ITest
{
    private readonly Test2 _t2;

    public Test1(Test2 t2)
    {
        _t2 = t2;
    }

    public void Do()
    {
        _t2.Do();
    }
}

[Registerable]
public class Test2
{
    public void Do()
    {
        Console.WriteLine("Hello world!");
    }
}

var host = new ServiceHost();
host.RegisterFromAssemblies(); // 从程序集注入（配合Registerable特性）
host.OnHostStarted += provider =>
{
    provider.GetService<ITest>().Do();
};
await host.RunAsync();

// 结果输出
Hello world!
```

### 0x2 配置文件
提供多种配置方式，你可以将配置都写进同一个配置文件中，使用时通过指定路径获取对应配置；或者单独对某个模块创建一个配置文件。后者是我们推荐的。
```json
// Config.json
{
    "ID": 1,
    "Name": "DwGöing"
}
```
```c#
record struct Config
{
    public int ID { get; init; }
    public string Name { get; init; }
}

var host = new ServiceHost();
host.AddJsonConfiguration("Config.json", reloadOnChange: true);
host.OnHostStarted += provider =>
{
    var config = provider.ParseConfiguration<Config>();
    Console.WriteLine($"{config.ID} {config.Name}");
};
await host.RunAsync();

// 结果输出
1 DwGöing
```

### 0x3 插件
#### 0x3.1 Aop
```c#
public interface I
{
    int Do(int a, int b);
}

[Intercept(typeof(LoggerInterceptor))]
public class A : I
{
    public A() { }

    public int Do(int a, int b)
    {
        return a + b;
    }
}

[Intercept(typeof(LoggerInterceptor))]
public class B : I
{
    public B() { }

    public int Do(int a, int b)
    {
        return a * b;
    }
}

var host = new ServiceHost();
host.ConfigureContainer(builder =>
{
    builder.RegisterType<A>().As<I>().EnableInterfaceInterceptors();
    builder.RegisterType<B>().As<I>().EnableInterfaceInterceptors();
});
host.ConfigureLoggerInterceptor(invocation => (
    $"{invocation.TargetType.Name}InvokeLog",
    LogLevel.Debug,
    "\n========================================\n"
    + $"Method:\t{invocation.Method}\n"
    + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
    + $"Return:\t{invocation.ReturnValue}\n"
    + "========================================"
));
host.OnHostStarted += provider =>
{
    var x = ServiceHost.ParseConfiguration<string>("ConnectionString");
    var y = provider.GetServices<I>();
    foreach (var item in provider.GetServices<I>()) item.Do(5, 6);
};
await host.RunAsync();

// 结果输出
========================================
Method: Int32 Do(Int32, Int32)
Args:   5|6
Return: 11
======================================== 
========================================
Method: Int32 Do(Int32, Int32)
Args:   5|6
Return: 30
========================================
```

#### 0x3.2 NLog
```xml
<!-- NLog.config示例 -->
<?xml version="1.0" encoding="utf-8"?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <targets>
    <!--使用可自定义的着色将日志消息写入控制台-->
    <target name="ColorConsole" xsi:type="ColoredConsole" layout="[${level}] ${date:format=yyyy\-MM\-dd HH\:mm\:ss}:${message} ${exception:format=message}" />
    <!--此部分中的所有目标将自动异步-->
    <target name="AsyncFile" xsi:type="AsyncWrapper">
      <target name="LogFile" xsi:type="File"
          fileName="${basedir}/Logs/Current.log"
          layout="[${level}] ${longdate} | ${message} ${onexception:${exception:format=message} ${newline} ${stacktrace} ${newline}"
          deleteOldFileOnStartup="true"
          archiveFileName="${basedir}/Logs/{#}.log"
          archiveNumbering="Date"
          archiveEvery="Day"
          maxArchiveFiles="7"
          concurrentWrites="true"
          keepFileOpen="false" />
    </target>
  </targets>
  <!--规则配置,final - 最终规则匹配后不处理任何规则-->
  <rules>
    <logger name="*" minlevel="Debug" writeTo="ColorConsole" /> 
    <logger name="*" minlevel="Info" writeTo="AsyncFile" />
    <logger name="Microsoft.*" minlevel="Info" writeTo="" final="true" />
  </rules>
</nlog>
```
```c#
host.ConfigureLogging(builder => builder.UserNLog("NLog.config"));
```

#### 0x3.3 Cache
```c#
var host = new ServiceHost();
host.ConfigureMemoryCache();
host.OnHostStarted += provider =>
{
    var cache = provider.GetCache();
    cache.Set("key", new { Name = "DwGöing" });
    Console.WriteLine(cache.Get("key"));
};
await host.RunAsync();

// 结果输出
{ Name = DwGöing }
```