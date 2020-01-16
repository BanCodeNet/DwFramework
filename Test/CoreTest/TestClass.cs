﻿using System;

using AutoFac.Extras.NLog.DotNetCore;
using Microsoft.Extensions.Configuration;

using DwFramework.Core.Models;

namespace Test
{
    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass1 : ITestInterface
    {
        public TestClass1(IConfiguration configuration, ILogger logger)
        {
            logger.Info("TestClass1已注入");
        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass1:{str}");
        }
    }

    [Registerable(typeof(ITestInterface), Lifetime.Singleton)]
    public class TestClass2 : ITestInterface
    {
        public TestClass2(IConfiguration configuration)
        {

        }

        public void TestMethod(string str)
        {
            Console.WriteLine($"TestClass2:{str}");
        }
    }
}
