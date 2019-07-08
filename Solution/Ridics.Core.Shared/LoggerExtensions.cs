using System;
using Microsoft.Extensions.Logging;

namespace Ridics.Core.Shared
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }

        public static void LogWarning(this ILogger logger, Exception ex)
        {
            logger.LogWarning(ex, ex.Message);
        }

        public static void LogCritical(this ILogger logger, Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }

        public static void LogDebug(this ILogger logger, Exception ex)
        {
            logger.LogDebug(ex, ex.Message);
        }

        public static void LogInformation(this ILogger logger, Exception ex)
        {
            logger.LogInformation(ex, ex.Message);
        }
    }
}