using Microsoft.AspNetCore.Mvc.Filters;

namespace Ridics.Authentication.Service.Authentication.Filters
{
    public class XFrameOptionsHeaderFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
            {
                context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            }
        }
    }
}