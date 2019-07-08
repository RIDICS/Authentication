using System.Collections.Generic;
using System.Security.Claims;
using Ridics.Authentication.Modules.Shared.Model;

namespace Ridics.Authentication.Modules.Shared
{
    public interface IExternalLoginProviderManager
    {
        bool CanCreateUser { get; }

        bool CanProvideEmail { get; }

        string ExtractEmail(IList<Claim> claims);

        ExternalLoginProviderUserModel MapClaims(IList<Claim> claims);
    }
}
