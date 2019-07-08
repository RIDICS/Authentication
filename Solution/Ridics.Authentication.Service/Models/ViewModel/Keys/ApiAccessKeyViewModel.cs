using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Keys
{
    public class ApiAccessKeyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "permissions")]
        public IList<ApiAccessPermissionEnumViewModel> Permissions { get; set; }

        public ApiAccessKeyHashViewModel ApiAccessKeyHashViewModel { get; set; }
    }
}