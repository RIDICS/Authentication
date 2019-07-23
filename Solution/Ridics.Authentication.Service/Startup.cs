using System;
using DryIoc;
using DryIoc.Facilities.AutoTx.Extensions;
using DryIoc.Facilities.NHibernate;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core;
using Ridics.Authentication.Service.DynamicModule;
using Ridics.Authentication.Service.IoC;
using Ridics.Core.Service.Shared.IoC;

namespace Ridics.Authentication.Service
{
    public class Startup
    {
        private readonly IConfiguration m_configuration;
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IHostingEnvironment m_environment;
        private IContainer m_container;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            IHostingEnvironment environment
        )
        {
            m_configuration = configuration;
            m_loggerFactory = loggerFactory;
            m_environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var dynamicModuleProvider = services.LoadModules(m_configuration, m_environment, m_loggerFactory);

            services.RegisterIdentity(m_configuration, m_loggerFactory);

            services.RegisterLocalization(m_configuration, "Localization");

            services.RegisterMvc(m_environment)
                .LoadDynamicControllers(dynamicModuleProvider);

            services.RegisterApiVersioning();

            services.RegisterSwagger();

            services.RegisterAuthorizationCore();

            services.RegisterAutomapper();

            services.RegisterAndConfigureOptions(m_configuration, m_environment);

            services.ConfigureReverseProxyHeaders(m_configuration);

            m_container = new Container()
                .WithDependencyInjectionAdapter(services,
                    throwIfUnresolved: type => type.Name.EndsWith("Controller")
                );

            try
            {
                m_container.RegisterAllComponents();
                m_container.AddAutoTx();
                m_container.AddNHibernate();
            }
            catch (Exception ex) // Any exception can be thrown including exception from database driver, e.g. SqlException
            {
                m_loggerFactory.CreateLogger<Startup>().LogCritical(ex, "Unable to initialize NHibernate using Auto Transactions");
                throw;
            }

            return m_container.Resolve<IServiceProvider>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            //loggerFactory.AddNLog(); NLog is registered in WebHostBuild in Program class

            app.ConfigureAutoMapper(cfg =>
            {
                cfg.CreateMissingTypeMaps = false;
            });

            // This constant is used to enable environment without trusted certificate (e.g. Linux)
            if (Environment.GetEnvironmentVariable("ASPNETCORE_DISABLE_HTTPS_REDIRECT") != "true")
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.ModuleConfigureLocalization();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            app.ConfigureSwagger(provider);

            app.ApplicationServices.GetService<DynamicModuleConfigurationManager>()
                .DumpConfigurationAndRestartIfChanged();

            app.ApplicationServices.GetService<DynamicModuleExternalLoginSynchronization>()
                .Synchronize();

            m_container.ResetAutoTxActivityContext();
        }
    }
}