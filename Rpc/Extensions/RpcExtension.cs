﻿using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Rpc.Extensions
{
    public static class RpcExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRpcService(this ServiceHost host)
        {
            host.RegisterType<RpcService>().SingleInstance();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitRpcServiceAsync(this IServiceProvider provider)
        {
            return provider.GetService<RpcService>().OpenServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RpcService GetRpcService(this IServiceProvider provider)
        {
            return provider.GetService<RpcService>();
        }
    }
}
