using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Ridics.Authentication.Service.Authentication.Filters
{
    public class ContentSecurityPolicyHeaderFilter : ActionFilterAttribute
    {
        private readonly ContentSecurityPolicyFilterConfiguration m_configuration;

        public ContentSecurityPolicyHeaderFilter(IOptions<ContentSecurityPolicyFilterConfiguration> configuration)
        {
            m_configuration = configuration.Value;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;

            if (result is ViewResult)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var cspBuilder = new StringBuilder();

                cspBuilder.Append("default-src 'self';");
                cspBuilder.Append(" object-src 'none';");

                cspBuilder.Append(m_configuration.EnableFontData ? " font-src 'self' data:;" : " font-src 'self';");

                cspBuilder.Append(m_configuration.EnableImageData ? " img-src 'self' data:;" : " img-src 'self';");

                cspBuilder.Append(" script-src  'self' https://www.google.com https://www.gstatic.com/ ;"); //HACK load from configuration
                cspBuilder.Append(" frame-src  'self' https://www.google.com ;"); //HACK load from configuration

                cspBuilder.Append(" frame-ancestors 'none';");
                cspBuilder.Append(" sandbox allow-forms allow-same-origin allow-scripts;");
                cspBuilder.Append(" base-uri 'self';");

                // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
                //cspBuilder.Append(" upgrade-insecure-requests;");

                var csp = cspBuilder.ToString();
                
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                }

                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                }
            }
        }
    }
}