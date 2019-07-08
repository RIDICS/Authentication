using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.ExternalIdentity
{
    public interface IConcreteExternalIdentityResolver
    {
        IList<UserExternalIdentityModel> Resolve(IList<UserDataModel> userDataList, IList<ExternalIdentityModel> externalIdentities);
    }
}