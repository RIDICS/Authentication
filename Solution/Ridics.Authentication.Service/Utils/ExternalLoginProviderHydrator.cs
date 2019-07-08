using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Utils;
using Ridics.Authentication.Service.Models;

namespace Ridics.Authentication.Service.Utils
{
    public class ExternalLoginProviderHydrator : IExternalLoginProviderHydrator
    {
        private readonly DynamicModuleProvider m_dynamicModuleProvider;

        public ExternalLoginProviderHydrator(DynamicModuleProvider dynamicModuleProvider)
        {
            m_dynamicModuleProvider = dynamicModuleProvider;
        }

        public void Hydrate(ExternalLoginProviderModel externalLoginProviderModel)
        {
            var configuration = m_dynamicModuleProvider.GetModuleConfigurations()
                .FirstOrDefault(x => x.Name == externalLoginProviderModel.AuthenticationScheme);

            externalLoginProviderModel.Enable = configuration != null && configuration.Enable;
        }
    }
}