using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.Utils
{
    public interface IExternalLoginProviderHydrator
    {
        void Hydrate(ExternalLoginProviderModel externalLoginProviderModel);
    }
}
