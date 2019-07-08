using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class PermissionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "roles")]
        public List<RoleViewModel> Roles { get; set; }
    }
}