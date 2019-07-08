using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Core.Configuration;
using Microsoft.Extensions.Options;
using Ridics.Authentication.DataEntities.Validators.Config;
using Ridics.Authentication.Service.Configuration;
using Ridics.Core.ReCaptcha.Config;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocConfigureOptionsExtensions
    {
        public static void RegisterAndConfigureOptions(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment
        )
        {
            services.AddConfigOptions<DynamicModuleConfiguration>(configuration, "Modules");
            services.AddConfigOptions<ExternalIdentityResolveConfiguration>(configuration, "ExternalIdentityResolveConfig");
            services.AddConfigOptions<PathConfiguration>(
                c => c.WebRootPath = environment.WebRootPath
            );
            services.AddConfigOptions<ReCaptchaConfig>(configuration, "GoogleReCaptcha");
            services.AddReloadableConfigOptions<ReturnUrlConfiguration>(configuration, "ReturnUrlConfig");
            services.AddConfigOptions<ValidatorConfig>(configuration, "ValidatorConfig");
            services.AddConfigOptions<LoginExpirationConfig>(configuration, "LoginExpiration");
            services.AddConfigOptions<TokensExpirationConfiguration>(configuration, "TokensExpiration");
            services.AddConfigOptions<FeatureFlagsConfiguration>(configuration, "FeatureFlags");
        }

        private static IServiceCollection AddConfigOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions)
            where TOptions : class, new()
        {
            services.Configure(configureOptions);
            services.AddConfig<TOptions>();
            return services;
        }

        private static IServiceCollection AddConfigOptions<TOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string section
        ) where TOptions : class, new()
        {
            services.Configure<TOptions>(configuration.GetSection(section));
            services.AddConfig<TOptions>();
            return services;
        }

        private static IServiceCollection AddReloadableConfigOptions<TOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            string section
        ) where TOptions : class, new()
        {
            services.Configure<TOptions>(configuration.GetSection(section));
            services.AddReloadableConfig<TOptions>();
            return services;
        }

        private static IServiceCollection AddConfig<TOptions>(this IServiceCollection services) where TOptions : class, new()
        {
            services.AddSingleton(cfg => cfg.GetService<IOptions<TOptions>>().Value);
            return services;
        }

        private static IServiceCollection AddReloadableConfig<TOptions>(this IServiceCollection services) where TOptions : class, new()
        {
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<TOptions>>().Value);
            return services;
        }
    }
}
