using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Helpers;

namespace Ridics.Authentication.Service.Models.ViewModel.Clients
{
    public class UriViewModel
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        [Display(Name = "uri")]
        [Required(ErrorMessage = "uri-required")]
        [CustomUri(ErrorMessage = "not-valid-uri")]
        public string Value { get; set; }

        [Display(Name = "is-redirect")]
        [UIHint("_BoolToStringTemplate")]
        public bool IsRedirect { get; set; }

        [Display(Name = "is-post-logout-redirect")]
        [UIHint("_BoolToStringTemplate")]
        public bool IsPostLogoutRedirect { get; set; }

        [Display(Name = "is-cors-origin")]
        [UIHint("_BoolToStringTemplate")]
        public bool IsCorsOrigin { get; set; }

        [Display(Name = "is-front-channel-logout")]
        [UIHint("_BoolToStringTemplate")]
        public bool IsFrontChannelLogout { get; set; }
    }
}