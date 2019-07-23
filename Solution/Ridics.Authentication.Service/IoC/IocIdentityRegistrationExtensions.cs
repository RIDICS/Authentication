using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Service.Authentication.Identity.Factories;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authentication.Identity.Stores;
using Ridics.Authentication.Service.Authentication.Identity;
using Ridics.Authentication.Service.Authentication.Identity.TokenProviders;
using Ridics.Authentication.Service.Authentication.Services;
using Ridics.Authentication.Service.Authentication.Stores;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Authorization.Handlers;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Controllers.API;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.TicketStore;
using Ridics.Authentication.TicketStore.Store;
using Scalesoft.Localization.AspNetCore;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocIdentityRegistrationExtensions
    {
        public static void RegisterIdentity(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            services.RegisterIdentitySystemComponents();
            services.RegisterIdentityServerComponents();

            services.RegisterIdentitySystem();
            services.RegisterIdentityServer(configuration, loggerFactory);

            services.RegisterAuthentication(configuration);
            services.RegisterAuthorization();

            services.ConfigureTokensExpiration(configuration);
        }

        private static void RegisterIdentitySystemComponents(this IServiceCollection services)
        {
            services.AddScoped<IdentitySignInManager>();
            services.AddScoped<IdentityUserManager>();
            services.AddScoped<IdentityRoleManager>();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory>();
            services.AddScoped<IRoleStore<ApplicationRole>, ApplicationRoleStore>();
            services.AddScoped<ILookupNormalizer, LookupNormalizer>();
            services.AddScoped<UserStore>();
            services.AddScoped<SmsTokenProvider>();
            services.AddScoped<EmailTokenProvider>();
            services.AddScoped<AuthenticatorTokenProvider>();
            services.AddScoped<TwoFactorTokenValidator>();
            services.AddScoped<MessageSenderTokenProviderAssociationManager>();
            services.AddScoped<PasswordResetTokenValidator>();
        }

        private static void RegisterIdentityServerComponents(this IServiceCollection services)
        {
            services.AddScoped<IPersistedGrantStore, PersistedGrantStore>();
            services.AddScoped<IAuthorizationHandler, EditUserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, ApiEditUserAuthorizationHandler>();
        }

        private static void RegisterIdentitySystem(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = PasswordRequirements.RequireDigit;
                    options.Password.RequireLowercase = PasswordRequirements.RequireLowercase;
                    options.Password.RequireUppercase = PasswordRequirements.RequireUppercase;
                    options.Password.RequireNonAlphanumeric = PasswordRequirements.RequireNonAlphanumeric;
                    options.Password.RequiredUniqueChars = 1; //required min count of unique chars
                    options.Password.RequiredLength = PasswordRequirements.MinLength; //required minimal length

                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;

                    options.Tokens.PasswordResetTokenProvider = PasswordResetTokenProvider.ProviderName;
                })
                .AddUserStore<UserStore>()
                .AddUserManager<IdentityUserManager>()
                .AddSignInManager<IdentitySignInManager>()
                .AddRoleManager<IdentityRoleManager>()
                .AddTokenProvider<EmailTokenProvider>(TokenOptions.DefaultProvider) //Default provider for all use cases when token provider is needed and no specific provider is registered
                .AddTokenProvider<EmailTokenProvider>(EmailTokenProvider.ProviderName)
                .AddTokenProvider<SmsTokenProvider>(SmsTokenProvider.ProviderName)
                .AddTokenProvider<PasswordResetTokenProvider>(PasswordResetTokenProvider.ProviderName)
                .AddTokenProvider<AuthenticatorTokenProvider>(AuthenticatorTokenProvider.ProviderName)
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(nameof(DataProtectorTokenProvider<ApplicationUser>))
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();
        }

        private static void RegisterIdentityServer(
            this IServiceCollection services,
            IConfiguration configuration,
            ILoggerFactory loggerFactory
        )
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var signingCredentialCertificatePath = configuration.GetSection("IdentityServer:SigningCredential").Value;


            //For custom options config see https://github.com/IdentityServer/IdentityServer4/blob/dev/src/IdentityServer4/Configuration/DependencyInjection/Options/IdentityServerOptions.cs
            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;

                options.UserInteraction.ErrorUrl = "/Error/500";
            });

            if (!string.IsNullOrEmpty(signingCredentialCertificatePath))
            {
                var signingCredentialCertificatePassword = configuration.GetSection("IdentityServer:SigningCredentialPassword").Value;
                
                try
                {
                    var signingCredentialCertificate = new X509Certificate2(signingCredentialCertificatePath, signingCredentialCertificatePassword);
                    identityServerBuilder.AddSigningCredential(signingCredentialCertificate);
                }
                catch (Exception exception) // expected type is probably WindowsCryptographicException
                {
                    var logger = loggerFactory.CreateLogger<X509Certificate2>();
                    logger.LogCritical(exception, "Unable to load Signing Credential certificate");
                    throw;
                }
            }
            else
            {
                var logger = loggerFactory.CreateLogger<X509Certificate2>();

                if (logger.IsEnabled(LogLevel.Critical))
                {
                    logger.LogCritical("Application is using DeveloperSigningCredential, configure IdentityServer:SigningCredential");
                }

                identityServerBuilder.AddDeveloperSigningCredential();
            }

            identityServerBuilder.AddClientStore<ClientStore>()
                .AddCorsPolicyService<CorsPolicyService>()
                .AddResourceStore<ResourceStore>()
                .AddJwtBearerClientAuthentication()
                .AddAppAuthRedirectUriValidator()
                .AddClientConfigurationValidator<DefaultClientConfigurationValidator>()
                .AddAspNetIdentity<ApplicationUser>();
        }

        private static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtAuthority = configuration.GetSection("JwtAuthentication:Authority").Value;
            var expirationConfig = configuration.GetSection("LoginExpiration").Get<LoginExpirationConfig>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = jwtAuthority;
                    options.Audience = "auth_api";

                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                })
                .Services.ConfigureApplicationCookie(options =>
                {
                    //Set login expiration timeout
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromSeconds(expirationConfig.TimeInSeconds);

                    options.AccessDeniedPath = "/Error/403";

                    // Only implement method, other methods are implemented with ASP Identity in CookieAuthenticationEvents (do not implement whole class)
                    options.Events.OnRedirectToLogin = context =>
                    {
                        //If request was to API, generate 401 response: https://devblog.dymel.pl/2016/07/07/return-401-unauthorized-from-asp-net-core-api/
                        if (context.Request.Path.StartsWithSegments(ApiControllerBase.ApiRouteStart) && context.Response.StatusCode == 200)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return Task.CompletedTask;
                        }

                        //Add culture to login request
                        var culture = context.HttpContext.RequestServices.GetRequiredService<ILocalizationService>().GetRequestCulture();
                        var redirectUriWithCultureParameter = QueryHelpers.AddQueryString(context.RedirectUri, "culture", culture.Name);

                        context.RedirectUri = redirectUriWithCultureParameter;
                        context.HttpContext.Response.Redirect(redirectUriWithCultureParameter);

                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        //If request was to API, generate 403 response: https://devblog.dymel.pl/2016/07/07/return-401-unauthorized-from-asp-net-core-api/
                        if (context.Request.Path.StartsWithSegments(ApiControllerBase.ApiRouteStart) && context.Response.StatusCode == 200)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            return Task.CompletedTask;
                        }

                        context.HttpContext.Response.Redirect(options.AccessDeniedPath);
                        return Task.CompletedTask;
                    };
                });

            //store claims in memory:
            services.SetAuthenticationTicketStore<MemoryCacheTicketStore>(new CacheTicketStoreConfig
            {
                SlidingExpiration = TimeSpan.FromSeconds(expirationConfig.TimeInSeconds)
            });
        }

        private static void RegisterAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.ViewAuthServiceAdministrationPolicy,
                    builder => builder.RequireClaim(CustomClaimTypes.Permission, PermissionNames.CanViewAuthServiceAdministration)
                );

                options.AddPolicy(PolicyNames.RestartAuthServicePolicy,
                    builder => builder.RequireClaim(CustomClaimTypes.Permission, PermissionNames.CanRestartAuthService)
                );

                options.AddPolicy(PolicyNames.InternalApiPolicy, policy =>
                    policy.RequireClaim(CustomClaimTypes.Scope, "auth_api.Internal"));

                options.AddPolicy(PolicyNames.NonceApiPolicy, policy =>
                    policy.RequireClaim(CustomClaimTypes.Scope, "auth_api.Nonce"));
            });

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }

        private static void ConfigureTokensExpiration(this IServiceCollection services, IConfiguration configuration)
        {
            var tokenExpirationConfig = configuration.GetSection("TokensExpiration").Get<TokensExpirationConfiguration>();

            services.Configure<CookieAuthenticationOptions>(IdentityConstants.TwoFactorUserIdScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromSeconds(tokenExpirationConfig.TwoFactorTokenExpirationInSeconds);
            });
        }
    }
}
