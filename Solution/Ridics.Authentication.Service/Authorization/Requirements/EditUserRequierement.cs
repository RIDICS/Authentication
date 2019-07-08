using Microsoft.AspNetCore.Authorization;

namespace Ridics.Authentication.Service.Authorization.Requirements
{
    public class EditUserRequirement : IAuthorizationRequirement
    {
        public EditUserRequirement(string role, int userId)
        {
            Role = role;
            UserId = userId;
        }

        public string Role { get; }

        public int UserId { get; }
    }
}