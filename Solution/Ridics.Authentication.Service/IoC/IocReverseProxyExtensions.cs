using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.Configuration;
using Ridics.Core.Service.Shared.IoC;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocReverseProxyExtensions
    {
        public static void ConfigureReverseProxyHeaders(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ReverseProxy").Get<ReverseProxyConfiguration>();
            services.ConfigureReverseProxyHeaders(config.IsDefault, config.ForwardLimit);
        }
    }
}
