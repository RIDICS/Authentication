using Microsoft.AspNetCore.Authorization;

namespace Ridics.Authentication.Service.Authorization.Requirements
{
    public class ApiEditUserRequirement : IAuthorizationRequirement
    {
        public bool CanOtherUserEdit { get; set; }

        public string PermissionRequiredToEditOtherUser { get; set; }

        public bool NeedPermissionToEditSelf { get; set; } = true;

        public string PermissionRequiredToEditSelf { get; set; }

        public int UserId { get; set; }
    }
}