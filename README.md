# DwFramework

### 0x1 项目简介

基于Autofac的Dotnet Core快速开发框架，这个框架旨在将服务注入简单化，把Autofac中常用的部分暴露出来，并融合了其他几个项目开发常用的组件。让整个开发的过程变得简单快速，（不能说学习是浪费时间，只是说有时候需要快速完成开发🤦‍♂️）。当然，如果你有更复杂的业务需求，你可以直接引用Autofac来对本框架进行扩展。

在框架的设计方面，在DDD的基础上使用者可以为单个服务使用不同的框架设计，创建一个立体化的DDD模型。下层框架（单个服务中的框架）中可以通过IServiceProvider来获取上层框架的服务，而反过来是不行的。这样的设计是为了实现基础服务共享，高级服务隔离的效果。

---

### 0x2 组件列表

版本说明：SolutionVersion.ReleaseVersion

|            组件             |     说明      |                             版本                             |
| :-------------------------: | :-----------: | :----------------------------------------------------------: |
|      DwFramework.Core       |   核心组件    | [![](https://img.shields.io/badge/Nuget-2.0.1.12-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Core/) |
|    DwFramework.Database     |    ORM组件    | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Database/) |
|    DwFramework.DataFlow     | 流式计算组件  | [![](https://img.shields.io/badge/Nuget-2.0.1.4-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.DataFlow/) |
| DwFramework.MachineLearning | 机器学习组件  | [![](https://img.shields.io/badge/Nuget-2.0.1.4-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.MachineLearning/) |
|    DwFramework.RabbitMQ     | RabbitMQ组件  | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |
|       DwFramework.Rpc       |    Rpc组件    | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Rpc/) |
|     DwFramework.Socket      |  Socket组件   | [![](https://img.shields.io/badge/Nuget-2.0.1.5-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Socket/) |
|  DwFramework.TaskSchedule   | 任务调度组件  | [![](https://img.shields.io/badge/Nuget-2.0.1.4-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.TaskSchedule/) |
|     DwFramework.WebAPI      |  WebAPI组件   | [![](https://img.shields.io/badge/Nuget-2.0.1.7-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebAPI/) |
|    DwFramework.WebSocket    | WebSocket组件 | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.WebSocket/) |

|             插件             |     说明     |                             状态                             |
| :--------------------------: | :----------: | :----------------------------------------------------------: |
| DwFramework.Plugins.Database |   ORM插件    | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.Database/) |
|   DwFramework.Plugins.Rpc    |   Rpc插件    | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.Rpc/) |
|  DwFramework.Plugins.WebAPI  |  WebAPI插件  | [![](https://img.shields.io/badge/Nuget-2.0.1.10-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.Plugins.WebAPI/) |
|     DwFramework.RabbitMQ     | RabbitMQ组件 | [![](https://img.shields.io/badge/Nuget-2.0.1.6-brightgreen.svg)](https://www.nuget.org/packages/DwFramework.RabbitMQ/) |

---

### 0x3 简单示例

```c#
// Test.cs
using System;
using Microsoft.Extensions.Configuration;
using DwFramework.Core.Models;

namespace Test
{
    public interface ITestInterface
    {
        void TestMethod(string str);
    }
  
    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        public TestClass1()
        {
            Console.WriteLine("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        public TestClass2()
        {
            Console.WriteLine("TestClass2已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }
}
```

```c#

// Program.cs
using DwFramework.Core;
using DwFramework.Core.Extensions;

class Program
{
    static void Main(string[] args)
    {
        ServiceHost host = new ServiceHost();
        host.RegisterFromAssembly("Test"); // 从程序集注入
        host.InitService(provider=>{
            var service = provider.GetService<ITest>();
            service.A("Test");
        });
      	host.Run();
    }
}
```