﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DwFramework.Core.Helper
{
    public static class TaskManager
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task CreateTask(Action taskAction)
        {
            var tcs = new TaskCompletionSource<object>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    taskAction();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task CreateTask(Action<CancellationToken> taskAction, out CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<object>();
            Task.Factory.StartNew(token =>
            {
                try
                {
                    taskAction((CancellationToken)token);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, cancellationToken.Token);
            return tcs.Task;
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<T> taskAction)
        {
            var tcs = new TaskCompletionSource<T>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    tcs.SetResult(taskAction());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 创建可取消任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="taskAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<T> CreateTask<T>(Func<CancellationToken, T> taskAction, CancellationTokenSource cancellationToken)
        {
            cancellationToken = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<T>();
            Task.Factory.StartNew(token =>
            {
                try
                {
                    tcs.SetResult(taskAction((CancellationToken)token));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, cancellationToken.Token);
            return tcs.Task;
        }
    }
}
