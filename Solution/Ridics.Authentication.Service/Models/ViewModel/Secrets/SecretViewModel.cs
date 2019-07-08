using System;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Secrets
{
    public class SecretViewModel
    {
        public int Id { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "value")]
        [Required(ErrorMessage = "value-required")]
        public string Value { get; set; }

        [Display(Name = "expiration")]
        public DateTime? Expiration { get; set; }
    }
}