using System;
using DryIoc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ridics.Core.Service.Shared.IoC
{
    public static class SharedIocExtensions
    {
        /// <summary>
        /// Configure IoC to be able resolve ILogger without specified generic type
        /// </summary>
        /// <param name="c">IoC interface for registration</param>
        public static void RegisterLogger(this IRegistrator c)
        {
            c.Register(Made.Of(
                () => DirectLoggerFactory.CreateLogger(Arg.Of<ILoggerFactory>(), Arg.Index<Type>(0)),
                request => request.Parent.ImplementationType));
        }

        /// <summary>
        /// Configure the app to handle X-Forwarded-Proto headers from proxy server.
        /// (Localhost IIS doesn't require this but the app can be deployed behind another reverse proxy on different server)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="isDefault">Use default configuration</param>
        /// <param name="forwardLimit">Max number of forwards</param>
        public static void ConfigureReverseProxyHeaders(this IServiceCollection services, bool isDefault, int forwardLimit)
        {
            if (isDefault)
                return;

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardLimit = forwardLimit;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear(); // Allow any reverse-proxy server
            });
        }
    }
}