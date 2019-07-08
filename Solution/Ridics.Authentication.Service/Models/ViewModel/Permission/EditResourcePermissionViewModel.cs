using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class EditResourcePermissionViewModel : ResourcePermissionViewModel
    {
        public IList<ResourcePermissionTypeActionViewModel> ResourcePermissionTypeActionList { get; set; }

        [Display(Name = "resource-permission-type-action-selected")]
        [Required(ErrorMessage = "resource-permission-type-action-selected-required")]
        public int SelectedResourcePermissionTypeActionId { get; set; }
    }
}