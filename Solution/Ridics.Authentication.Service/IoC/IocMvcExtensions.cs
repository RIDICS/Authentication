using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.Authentication.Filters;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocMvcExtensions
    {
        public static IMvcCoreBuilder RegisterMvc(
            this IServiceCollection services,
            IHostingEnvironment hostingEnvironment
        )
        {
            services.Configure<CookieTempDataProviderOptions>(options => { options.Cookie.IsEssential = true; });
            
            services.Configure<ContentSecurityPolicyFilterConfiguration>(o =>
            {
                o.EnableFontData = hostingEnvironment.IsDevelopment();
                //TODO can be removed after resolving how to force bootstrap not to use data or custom icons will be used
                o.EnableImageData = true;
            });

            var mvcCoreBuilder=services.AddMvcCore(
                o =>
                {
                    o.Filters.Add<ContentSecurityPolicyHeaderFilter>();
                    o.Filters.Add<ReferrerPolicyHeaderFilter>();
                    o.Filters.Add<XContentTypeOptionsHeaderFilter>();
                    o.Filters.Add<XFrameOptionsHeaderFilter>();
                    o.Filters.Add<XXssProtectionHeaderFilter>();
                    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); //needs to be registered by instance
                }
            );

            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
            });

            return mvcCoreBuilder;
        }
    }
}
