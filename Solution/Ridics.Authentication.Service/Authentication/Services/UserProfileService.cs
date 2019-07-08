using System;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Ridics.Authentication.Core.Managers;

namespace Ridics.Authentication.Service.Authentication.Services
{
    [Obsolete("Identity server uses profile service from ASP Identity", true)]
    public class UserProfileService : IProfileService
    {
        private readonly UserManager m_userManager;

        public UserProfileService(UserManager userManager)
        {
            m_userManager = userManager;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (int.TryParse(context.Subject.GetSubjectId(), out var id))
            {
                var user = m_userManager.GetUserById(id);
                if (!user.HasError)
                {
                    //UserViewModel does not have GetClaims method anymore
                    //context.AddRequestedClaims(user.Result.GetClaims());
                }
            }

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            if (int.TryParse(context.Subject.GetSubjectId(), out var id))
            {
                var user = m_userManager.GetUserById(id);
                if (!user.HasError)
                {
                    context.IsActive = true; //TODO add IsActive to user
                }
            }

            return Task.CompletedTask;
        }
    }
}