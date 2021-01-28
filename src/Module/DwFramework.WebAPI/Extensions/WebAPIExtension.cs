﻿using System;
using System.Threading.Tasks;

using DwFramework.Core;

namespace DwFramework.WebAPI
{
    public static class WebAPIExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterWebAPIService<T>(this ServiceHost host, string path = null, string key = null) where T : class
        {
            host.Register(_ => new WebAPIService(path, key)).SingleInstance();
            host.OnInitializing += async provider => await provider.InitWebAPIServiceAsync<T>();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static WebAPIService GetWebAPIService(this IServiceProvider provider)
        {
            return provider.GetService<WebAPIService>();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        public static Task InitWebAPIServiceAsync<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetWebAPIService().OpenServiceAsync<T>();
        }
    }
}
