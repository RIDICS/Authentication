using DryIoc;
using DryIoc.Facilities.NHibernate;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Core.Utils;
using Ridics.Authentication.Service.Builders.Implementation;
using Ridics.Authentication.Service.Builders.Interface;
using Ridics.Authentication.Service.Controllers;
using Ridics.Authentication.Service.DynamicModule;
using Ridics.Authentication.Service.Factories;
using Ridics.Authentication.Service.Factories.Implementation;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Managers;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.SharedInterfaceImpl;
using Ridics.Authentication.Service.Utils;
using Ridics.Authentication.Shared;
using Ridics.Core.Service.Shared.IoC;
using Ridics.Core.ReCaptcha.Managers;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocComponentsRegistrationExtensions
    {
        public static void RegisterAllComponents(this IContainer container)
        {
            container.RegisterLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.RegisterComponents();
            serviceCollection.RegisterAttributeAdapters();

            container.Populate(serviceCollection);
        }

        private static void RegisterComponents(this IServiceCollection services)
        {
            services.AddSingleton<LiveManager>();

            services.AddSingleton<INHibernateInstaller, NHibernateInstaller>();

            services.AddScoped<DynamicModuleConfigurationManager>();
            services.AddScoped<DynamicModuleExternalLoginSynchronization>();

            services.AddScoped<ReCaptchaManager>();
            services.AddScoped<TwoFactorValidator>();
            services.AddScoped<TwoFactorProvidersProvider>();
            services.AddScoped<UserApiResultFactory>();

            services.AddScoped<IGenericViewModelFactory, GenericViewModelFactory>();
            services.AddScoped<IBasicViewModelFactory, BasicViewModelFactory>();
            services.AddScoped<IEditableViewModelFactory, EditableViewModelFactory>();
            services.AddScoped<IViewModelFactory, ViewModelFactory>();
            services.AddScoped<IViewModelBuilder, ViewModelBuilder>();
            services.AddScoped<FeatureFlagsManager>();
            services.AddScoped<LocalizationManager>();
            services.AddScoped<UserHelper>();

            services.AddSingleton<IExternalLoginProviderHydrator, ExternalLoginProviderHydrator>();

            services.AddScoped<IErrorHandler, ErrorController>();
            services.AddScoped<IDateTimeStringMapper, DateTimeStringMapperImpl>();
        }

        private static void RegisterAttributeAdapters(this IServiceCollection services)
        {
            services.AddSingleton<IValidationAttributeAdapterProvider, CustomUriAttributeAdapterProvider>();
        }
    }
}
