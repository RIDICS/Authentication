using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class ResourcePermissionTypeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "resource-permission-type-action")]
        public IList<ResourcePermissionTypeActionViewModel> ResourcePermissionTypeActions { get; set; }
    }
}