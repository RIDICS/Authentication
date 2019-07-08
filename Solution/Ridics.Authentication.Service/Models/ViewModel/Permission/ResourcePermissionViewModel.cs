using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Users;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class ResourcePermissionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "resource-id")]
        [Required(ErrorMessage = "resource-id-required")]
        public string ResourceId { get; set; }

        [Display(Name = "resource-permission-type-action")]
        public ResourcePermissionTypeActionViewModel ResourcePermissionTypeAction { get; set; }

        [ValidateNever]
        public string ResourcePermissionString => $"{ResourcePermissionTypeAction.ResourcePermissionType.Name} : {ResourceId} : {ResourcePermissionTypeAction.Name}";

        [Display(Name = "roles")]
        public List<RoleViewModel> Roles { get; set; }

        [Display(Name = "users")]
        public List<UserViewModel> Users { get; set; }
    }
}