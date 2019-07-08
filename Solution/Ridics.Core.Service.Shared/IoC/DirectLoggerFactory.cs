using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Ridics.Core.Service.Shared.IoC
{
    public static class DirectLoggerFactory
    {
        public static ILogger CreateLogger(ILoggerFactory loggerProvider, Type type)
        {
            return loggerProvider.CreateLogger(TypeNameHelper.GetTypeDisplayName(type));
        }
    }
}