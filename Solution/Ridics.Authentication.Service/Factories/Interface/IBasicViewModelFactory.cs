using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Factories.Interface
{
    public interface IBasicViewModelFactory
    {
        IList<ExternalLoginProviderViewModel> FindExternalProviderViewModels();

        LoginViewModel GetLoginViewModel(string returnUrl);

        ClaimTypeViewModel GetClaimTypeViewModel(IList<ClaimTypeEnumViewModel> allTypes);

        UserClaimsViewModel GetUserClaimsViewModel(IList<ClaimViewModel> userClaims, int userId, string dialogTitle, string dialogMessage);

        ApiResourceSecretsViewModel GetApiResourceSecretsViewModel(IList<SecretViewModel> secrets, int apiResourceId, string dialogTitle,
            string dialogMessage);

        ApiResourceScopesViewModel GetApiResourceScopesViewModel(IList<ScopeViewModel> scopes, int apiResourceId, string dialogTitle,
            string dialogMessage);

        ResourcePermissionTypeActionsViewModel GetPermissionTypeActionsViewModel(IList<ResourcePermissionTypeActionViewModel> actions, int resourcePermissionTypeId,
            string dialogTitle,
            string dialogMessage);

        UriViewModel GetUriViewModel(int clientId);

        ClientSecretsViewModel GetClientSecretsViewModel(IList<SecretViewModel> secrets, int clientId, string dialogTitle,
            string dialogMessage);

        ConsentViewModel GetConsentViewModel(string returnUrl, ClientViewModel client, IList<ScopeViewModel> scopes,
            IList<IdentityResourceViewModel> identityResources);
    }
}