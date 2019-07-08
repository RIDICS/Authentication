using System.Globalization;
using Microsoft.AspNetCore.Http;
using Scalesoft.Localization.AspNetCore.Manager;

namespace Ridics.Authentication.Service.SharedInterfaceImpl
{
    public class RequestCultureManagerImpl : IRequestCultureManager
    {
        public const string CultureQueryName = "culture";

        private readonly RequestCultureManager m_requestCultureManager;
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public RequestCultureManagerImpl(IHttpContextAccessor httpContextAccessor)
        {
            m_httpContextAccessor = httpContextAccessor;
            m_requestCultureManager = new RequestCultureManager(httpContextAccessor);
        }

        public CultureInfo ResolveRequestCulture(CultureInfo defaultCulture)
        {
            //First try to get culture from HttpContext.Items (i.e. during login)
            var contextCulture = m_httpContextAccessor.HttpContext.Items[CultureQueryName] as string;

            if (!string.IsNullOrEmpty(contextCulture))
            {
                return new CultureInfo(contextCulture);
            }

            //Second get culture from cookie or use default culture
            return m_requestCultureManager.ResolveRequestCulture(defaultCulture);
        }

        public void SetCulture(string culture)
        {
            m_requestCultureManager.SetCulture(culture);
        }
    }
}