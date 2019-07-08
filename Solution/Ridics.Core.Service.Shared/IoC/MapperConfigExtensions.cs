using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ridics.Core.Service.Shared.IoC
{
    public static class MapperConfigExtensions
    {
        public static void ConfigureAutoMapper(this IApplicationBuilder applicationBuilder, Action<IMapperConfigurationExpression> customCfg = null)
        {
            var serviceProvider = applicationBuilder.ApplicationServices;

            var profiles = serviceProvider.GetServices<Profile>();

            try
            {
                var mapper = Mapper.Instance;

                if (mapper != null)
                {
                    Mapper.Reset();
                }
            }
            catch (InvalidOperationException)
            {
                // not initialized yet, proceed normally
            }

            Mapper.Initialize(cfg =>
            {
                cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(serviceProvider, type));

                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }

                customCfg?.Invoke(cfg);
            });

            //Mapper.Configuration.AssertConfigurationIsValid(); //Use this for mapping debug
        }
    }
}