using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Users;

namespace Ridics.Authentication.Service.Models.ViewModel.Permission
{
    public class ResourcePermissionTypeActionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "resource-permission-type")]
        public ResourcePermissionTypeViewModel ResourcePermissionType { get; set; }

        [Display(Name = "roles")]
        public List<RoleViewModel> Roles { get; set; }

        [Display(Name = "users")]
        public List<UserViewModel> Users { get; set; }

        [ValidateNever]
        public string ResourcePermissionString => $"{ResourcePermissionType.Name}:{Name}";
    }
}