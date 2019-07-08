using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class ExternalLoginViewModel
    {
        public int Id { get; set; }

        public string ProviderKey { get; set; }

        public ExternalLoginProviderViewModel LoginProvider { get; set; }
    }
}