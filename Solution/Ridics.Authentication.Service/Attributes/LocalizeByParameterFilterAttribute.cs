using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.SharedInterfaceImpl;

namespace Ridics.Authentication.Service.Attributes
{
    public class LocalizeByParameterFilterAttribute : ActionFilterAttribute
    {
        private const string ReturnUrlParam = "returnUrl";
        private const string LogoutIdParam = "logoutId";
        private const string LoginCultureCookieName = "login.localizationCulture";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestCulture = GetRequestCulture(context.HttpContext);
            var cookieCulture = GetCookieCulture(context.HttpContext);

            if (!string.IsNullOrEmpty(requestCulture))
            {
                if (requestCulture != cookieCulture)
                {
                    context.HttpContext.Response.Cookies.Append(LoginCultureCookieName, requestCulture, new CookieOptions {IsEssential = true});
                }

                context.HttpContext.Items.Add(RequestCultureManagerImpl.CultureQueryName, requestCulture);
            }
            else if (!string.IsNullOrEmpty(cookieCulture))
            {
                context.HttpContext.Items.Add(RequestCultureManagerImpl.CultureQueryName, cookieCulture);
            }

            base.OnActionExecuting(context);
        }

        private string GetCookieCulture(HttpContext context)
        {
            var cultureCookie = context.Request.Cookies[LoginCultureCookieName];

            return cultureCookie;
        }

        private string GetRequestCulture(HttpContext context)
        {
            var request = context.Request;
            
            if (request.Query.TryGetValue(ReturnUrlParam, out var returnUrl) || (request.HasFormContentType && request.Form.TryGetValue(ReturnUrlParam, out returnUrl)))
            {
                if (TryGetCultureFromReturnUrl(context, returnUrl, out var culture))
                {
                    return culture;
                }
            }

            if (request.Query.TryGetValue(LogoutIdParam, out var logoutId))
            {
                if (TryGetCultureFromLogoutId(context, logoutId, out var culture))
                {
                    return culture;
                }
            }

            if (request.Query.TryGetValue(RequestCultureManagerImpl.CultureQueryName, out var queryCulture))
            {
                return queryCulture;
            }

            return null;
        }

        private bool TryGetCultureFromReturnUrl(HttpContext context, string returnUrl, out string culture)
        {
            var interaction = context.RequestServices.GetRequiredService<IIdentityServerInteractionService>();

            var authContext = interaction.GetAuthorizationContextAsync(returnUrl).GetAwaiter().GetResult();

            culture = authContext?.Parameters[RequestCultureManagerImpl.CultureQueryName];

            return !string.IsNullOrEmpty(culture);
        }

        private bool TryGetCultureFromLogoutId(HttpContext context, string returnUrl, out string culture)
        {
            var interaction = context.RequestServices.GetRequiredService<IIdentityServerInteractionService>();

            var logoutContext = interaction.GetLogoutContextAsync(returnUrl).GetAwaiter().GetResult();

            culture = logoutContext?.Parameters[RequestCultureManagerImpl.CultureQueryName];

            return !string.IsNullOrEmpty(culture);
        }
    }
}