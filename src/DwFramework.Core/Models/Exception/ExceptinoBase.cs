using System;
using System.Collections.Generic;

namespace DwFramework.Core
{
    public abstract class ExceptionBase : Exception
    {
        public static Dictionary<int, string> Codes = new()
        {
            { 401, "NotFoundException" }
        };

        public int Code { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ExceptionBase(int code, string message = null, Exception innerException = null)
            : base(Codes.TryGetValue(code, out var desc) ? $"{desc}{(!string.IsNullOrEmpty(message) ? $":{message}" : "")}" : (!string.IsNullOrEmpty(message) ? $"{message}" : null), innerException)
        {
            Code = code;
        }
    }
}