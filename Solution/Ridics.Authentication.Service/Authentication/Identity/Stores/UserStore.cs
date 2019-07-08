using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Exceptions;

namespace Ridics.Authentication.Service.Authentication.Identity.Stores
{
    public class UserStore : UserStore<ApplicationUser>
    {
        public UserStore(UserManager userManager, ExternalLoginManager externalLoginManager, RoleManager roleManager)
            : base(userManager, externalLoginManager, roleManager)
        {
        }
    }

    public class UserStore<TUser> :
        IUserPasswordStore<TUser>, IUserTwoFactorStore<TUser>,
        IUserEmailStore<TUser>, IUserPhoneNumberStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>, IUserRoleStore<TUser>,
        IUserLockoutStore<TUser>, IUserSecurityStampStore<TUser>,
        IUserLoginStore<TUser>
        where TUser : ApplicationUser
    {
        private readonly UserManager m_userManager;
        private readonly ExternalLoginManager m_externalLoginManager;
        private readonly RoleManager m_roleManager;

        public UserStore(UserManager userManager, ExternalLoginManager externalLoginManager, RoleManager roleManager)
        {
            m_userManager = userManager;
            m_externalLoginManager = externalLoginManager;
            m_roleManager = roleManager;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            var userModel = Mapper.Map<UserModel>(user);
            var result = m_userManager.CreateUser(userModel);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return Task.FromResult(IdentityResult.Failed(error));
            }

            user.Id = result.Result;
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            var userModel = Mapper.Map<UserModel>(user);
            var result = m_userManager.UpdateUser(user.Id, userModel);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return Task.FromResult(IdentityResult.Failed(error));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            var result = m_userManager.DeleteUserWithId(user.Id);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return Task.FromResult(IdentityResult.Failed(error));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = m_userManager.GetUserById(int.Parse(userId));
            var appUser = Mapper.Map<TUser>(result.Result);
            return Task.FromResult(appUser);
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = m_userManager.GetUserByUsername(normalizedUserName);
            var appUser = Mapper.Map<TUser>(result.Result);
            return Task.FromResult(appUser);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }


        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var result = m_userManager.GetUserByConfirmedEmail(normalizedEmail);
            var appUser = Mapper.Map<TUser>(result.Result);
            return Task.FromResult(appUser);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail,
            CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed,
            CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            user.AuthenticatorKey = key;
            return Task.CompletedTask;
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AuthenticatorKey);
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var result = m_userManager.AddRoleToUser(user.Id, roleName);

            if (!result.HasError)
            {
                return Task.CompletedTask;
            }

            return Task.FromException<IList<string>>(new StoreException(string.Format("Can not add role to user {0}", user.Id)));
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); //TODO implement
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            var result = m_roleManager.GetRolesByUser(user.Id);

            if (!result.HasError)
            {
                IList<string> roleNames = result.Result.Select(x => x.Name).ToList();
                return Task.FromResult(roleNames);
            }

            return Task.FromException<IList<string>>(new StoreException(string.Format("Can not get roles for user {0}", user.Id)));
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            var result = m_roleManager.GetRolesByUser(user.Id);

            if (!result.HasError)
            {
                var role = result.Result.FirstOrDefault(x => string.Equals(x.Name, roleName, StringComparison.CurrentCultureIgnoreCase));

                return Task.FromResult(role != null);
            }

            return Task.FromException<bool>(new StoreException(string.Format("Can not get roles for user {0}", user.Id)));
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(); //TODO implement
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount += 1;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            user.AccessFailedCount = 0;
            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            m_externalLoginManager.CreateExternalLogin(user.Id, null, login.ProviderKey);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            var result = m_userManager.GetUserByExternalLogin(loginProvider, providerKey);

            if (result.HasError)
            {
                return Task.FromResult<TUser>(null); //TODO investigate better handling
            }

            var appUser = Mapper.Map<TUser>(result.Result);
            return Task.FromResult(appUser);
        }
    }
}
