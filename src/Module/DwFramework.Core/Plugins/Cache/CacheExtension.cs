﻿using System;

using DwFramework.Core.Extensions;

namespace DwFramework.Core.Plugins
{
    public static class CacheExtension
    {
        /// <summary>
        /// 获取缓存服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ICache GetCache(this IServiceProvider provider)
        {
            return provider.GetService<ICache>();
        }
    }
}
