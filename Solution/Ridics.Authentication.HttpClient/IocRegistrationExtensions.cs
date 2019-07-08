using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Ridics.Authentication.HttpClient.Client;
using Ridics.Authentication.HttpClient.Client.Auth;
using Ridics.Authentication.HttpClient.Configuration;
using Ridics.Authentication.HttpClient.Provider;
using Ridics.Core.Shared.Providers;
using Ridics.Core.HttpClient.Authentication.Events;
using Ridics.Core.HttpClient.Authentication.Options;
using Ridics.Core.HttpClient.Authentication.Service;
using Ridics.Core.HttpClient.Client;
using Ridics.Core.HttpClient.Config;
using Ridics.Core.HttpClient.Provider;
using Ridics.Core.HttpClient.Storage;

namespace Ridics.Authentication.HttpClient
{
    public static class IocRegistrationExtensions
    {
        public static void RegisterAuthorizationHttpClientComponents<TClientLocalization>(this IServiceCollection services,
            AuthServiceCommunicationConfiguration configuration = null,
            OpenIdConnectConfig openIdConfiguration = null,
            AuthServiceControllerBasePathsConfiguration pathConfiguration = null)
            where TClientLocalization : class, IAuthorizationServiceClientLocalization
        {
            services.AddScoped<IAuthorizationServiceClientLocalization, TClientLocalization>();

            services.AddScoped<AuthorizationServiceHttpClient>();

            services.AddScoped<AuthServiceControllerBasePathsProvider>();

            services.AddScoped<RegistrationApiClient>();
            services.AddScoped<ExternalIdentityProviderApiClient>();
            services.AddScoped<FileResourceApiClient>();
            services.AddScoped<NonceApiClient>();
            services.AddScoped<UserApiClient>();
            services.AddScoped<RoleApiClient>();
            services.AddScoped<PermissionApiClient>();
            services.AddScoped<ContactApiClient>();
            services.TryAddScoped<IDateTimeProvider, DateTimeProvider>();
            services.TryAddScoped<AuthApiAccessTokenProvider>();

            services.TryAddSingleton<ITokenStorage, InMemoryTokenStorage>();

            services.TryAddTransient<ITokenEndpointClient, TokenEndpointClient>();
            services.TryAddSingleton<TokenEndpointHttpClientProvider>();

            if (configuration != null)
            {
                services.AddSingleton(configuration);
            }

            if (openIdConfiguration != null)
            {
                services.AddSingleton(openIdConfiguration);
            }

            if (pathConfiguration != null)
            {
                services.AddSingleton(pathConfiguration);
            }
        }

        public static void RegisterAutomaticTokenManagement(this IServiceCollection services)
        {
            services.AddTransient<AutomaticTokenManagementCookieEvents>();
            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, AutomaticTokenManagementConfigureCookieOptions>();
        }
    }
}
