using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Ridics.Authentication.Service.Controllers.API;
using Ridics.Authentication.Service.SharedInterfaceImpl;

namespace Ridics.Authentication.Service.Utils
{
    public class LocalizeApiByHeaderFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headerCulture = context.HttpContext.Request.Headers[ApiControllerBase.HeaderCulture].FirstOrDefault();

            if (!string.IsNullOrEmpty(headerCulture))
            {
                context.HttpContext.Items.Add(RequestCultureManagerImpl.CultureQueryName, headerCulture);
            }

            base.OnActionExecuting(context);
        }
    }
}