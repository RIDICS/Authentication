using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.SharedInterfaceImpl;
using Ridics.Authentication.Shared;
using Scalesoft.Localization.AspNetCore.IoC;
using Scalesoft.Localization.AspNetCore.Manager;
using Scalesoft.Localization.Core.Configuration;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocLocalizationRegistrationExtensions
    {
        public static void RegisterLocalization(
            this IServiceCollection services,
            IConfiguration configuration,
            string configurationKey
        )
        {
            var localizationConfiguration = configuration.GetSection(configurationKey).Get<LocalizationConfiguration>();

            services.AddSingleton<IRequestCultureManager, RequestCultureManagerImpl>(); // Must be a singleton, because ASP.NET Core uses singletons for localization component registration
            services.AddLocalizationService(localizationConfiguration);

            services.AddScoped<ITranslator, TranslatorImpl>();
            
            services.AddMvc().AddDataAnnotationsLocalization(
                options => { options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(type.Name, "File"); }
            );
        }
    }
}
