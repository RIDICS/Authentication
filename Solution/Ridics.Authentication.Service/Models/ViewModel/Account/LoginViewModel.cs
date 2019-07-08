using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Controllers;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public partial class LoginViewModel
    {
        [Required(ErrorMessage = "input-required")]
        public LoginFormModel Input { get; set; }

        public string Action { get; set; } = nameof(AccountController.Login);

        public bool LinkExternalIdentity { get; set; }

        public string ReturnUrl { get; set; }

        public string ReturnUrlOnCancel { get; set; }

        public IEnumerable<ExternalLoginProviderViewModel> ExternalProviders { get; set; }

        public ExternalProviderInfoLabelType? ExternalProviderInfoLabelType { get; set; }
    }
}
