using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.ExternalIdentity
{
    public interface IExternalIdentityResolver
    {
        IList<UserExternalIdentityModel> Resolve(UserModel user);
    }
}