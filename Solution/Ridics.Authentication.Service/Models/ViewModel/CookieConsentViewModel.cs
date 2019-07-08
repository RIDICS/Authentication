using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace Ridics.Authentication.Service.Models.ViewModel
{
    public class CookieConsentViewModel
    {
        [Display(Name = "essential-cookies-label")]
        public bool EssentialCookies { get; set; }
    }
}
