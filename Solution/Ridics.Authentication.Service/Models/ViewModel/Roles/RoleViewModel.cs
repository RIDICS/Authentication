using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.Models.ViewModel.Roles
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "name-required")]
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "permissions")]
        public List<PermissionViewModel> Permissions { get; set; }

        [Display(Name = "resource-permissions")]
        public List<ResourcePermissionViewModel> ResourcePermissions { get; set; }

        [Display(Name = "resource-permission-types")]
        public List<ResourcePermissionTypeActionViewModel> ResourcePermissionTypeActions { get; set; }

    }
}