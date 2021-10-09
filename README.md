# DwFramework
[![](https://img.shields.io/badge/%E6%9B%B4%E6%96%B0%E6%97%A5%E5%BF%97-v5.1.2-brightgreen)](https://github.com/BanCodeNet/DwFramework/blob/readme/ReleaseLog_v5.1.x.md)
![](https://github.com/DwGoingJiang/DwFramework/workflows/Ubuntu/badge.svg)
### 0x1 组件列表
|            组件             |     说明      |                             版本                             |
| :-------------------------: | :-----------: | :----------------------------------------------------------: |
|      DwFramework.Core       |   核心库    | [![](https://img.shields.io/badge/Nuget-5.1.2-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Core/) |
|    DwFramework.SqlSugar     |    SqlSugar封装库    | [![](https://img.shields.io/badge/Nuget-5.1.2-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.SqlSugar/) |
|    DwFramework.RabbitMQ     | RabbitMQ封装库  | [![](https://img.shields.io/badge/Nuget-5.1.2-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |
|       DwFramework.Quartz       |    Quartz封装库    | [![](https://img.shields.io/badge/Nuget-5.1.2-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Quartz/) |
|     DwFramework.Web      |  网络库   | [![](https://img.shields.io/badge/Nuget-5.1.2-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Web/) |
---
### 0x2 简单示例

```c#
class Program
{
    static async Task Main(string[] args)
    {
        var host = new ServiceHost();
        host.ConfigureLogging(builder => builder.UserNLog());
        host.RegisterFromAssemblies();
        host.OnHostStarted += provider =>
        {
            foreach (var item in provider.GetServices<I>())
                Console.WriteLine(item.Do(5, 6));
        };
        await host.RunAsync();
    }
}

// 定义接口
public interface I
{
    int Do(int a, int b);
}

// 定义实现
[Registerable(typeof(I))]
public class A : I
{
    public A() { }

    public int Do(int a, int b)
    {
        return a + b;
    }
}

// 定义实现
[Registerable(typeof(I))]
public class B : I
{
    public B() { }

    public int Do(int a, int b)
    {
        return a * b;
    }
}
```