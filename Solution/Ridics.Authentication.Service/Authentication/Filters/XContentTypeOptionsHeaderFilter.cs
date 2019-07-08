using Microsoft.AspNetCore.Mvc.Filters;

namespace Ridics.Authentication.Service.Authentication.Filters
{
    public class XContentTypeOptionsHeaderFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            }
        }
    }
}