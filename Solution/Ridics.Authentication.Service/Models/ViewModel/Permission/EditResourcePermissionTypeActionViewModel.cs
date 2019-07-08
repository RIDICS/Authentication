using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class EditResourcePermissionTypeActionViewModel : ResourcePermissionTypeActionViewModel
    {
        public IList<ResourcePermissionTypeViewModel> ResourcePermissionTypeList { get; set; }

        [Display(Name = "resource-permission-type-selected")]
        [Required(ErrorMessage = "resource-permission-type-selected-required")]
        public int SelectedResourcePermissionTypeId { get; set; }
    }
}