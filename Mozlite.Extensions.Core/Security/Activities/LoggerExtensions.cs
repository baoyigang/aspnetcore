﻿using System;
using Microsoft.Extensions.Logging;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 日志扩展类。
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// 记录用户日志。
        /// </summary>
        /// <param name="logger">当前日志接口。</param>
        /// <param name="message">用户操作信息。</param>
        /// <param name="args">格式化参数。</param>
        public static void LogUserInformation(this ILogger logger, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            logger.LogInformation(ActivityLogger.EventId, message, args);
        }
    }
}