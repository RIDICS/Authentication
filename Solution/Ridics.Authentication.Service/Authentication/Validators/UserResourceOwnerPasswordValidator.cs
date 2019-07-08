using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Ridics.Authentication.Service.Authentication.Identity.Managers;

namespace Ridics.Authentication.Service.Authentication.Validators
{
    [Obsolete("Identity server uses UserResourceOwnerPasswordValidator from ASP Identity", true)]
    public class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IdentityUserManager m_userManager;

        public UserResourceOwnerPasswordValidator(IdentityUserManager userManager)
        {
            m_userManager = userManager;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var appUser = m_userManager.GetUserByUsernameAsync(context.UserName).Result;

            var passwordMatch = m_userManager.CheckPasswordAsync(appUser, context.Password).Result;

            if (!passwordMatch) return Task.CompletedTask;

           /* UserViewModel does not have GetClaims method anymore
            var claims = Mapper.Map<UserViewModel>(appUser).GetClaims();
            
            context.Result = new GrantValidationResult(appUser.Id, "custom", claims);*/

            return Task.CompletedTask;
        }
    }
}