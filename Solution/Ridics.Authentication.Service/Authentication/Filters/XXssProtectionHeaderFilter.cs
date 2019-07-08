using Microsoft.AspNetCore.Mvc.Filters;

namespace Ridics.Authentication.Service.Authentication.Filters
{
    public class XXssProtectionHeaderFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
            if (!context.HttpContext.Response.Headers.ContainsKey("X-XSS-Protection"))
            {
                context.HttpContext.Response.Headers.Add("X-XSS-Protection", "1");
            }
        }
    }
}