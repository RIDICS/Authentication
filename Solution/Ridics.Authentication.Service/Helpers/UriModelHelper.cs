using System.Linq;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.Helpers
{
    public static class UriModelHelper
    {
        public const string Redirect = "Redirect";
        public const string PostLougoutRedirect = "PostLogoutRedirect";
        public const string CorsOrigin = "CorsOrigin";
        public const string FrontChannelLogout = "FrontChannelLogout";
        
        public static bool IsUriOfType(this UriModel uri, string type)
        {
            return uri.UriTypes.FirstOrDefault(x => x.UriTypeValue == type) != null;
        }
    }
}