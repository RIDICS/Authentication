using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Keys;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Factories.Interface
{
    public interface IEditableViewModelFactory
    {
        EditableClaimViewModel GetAddClaimViewModel(int userId, IList<ClaimTypeViewModel> claimTypes);

        EditableIdentityResourceViewModel GetEditableIdentityResourceViewModel(IList<ClaimTypeViewModel> claimTypes,
            IdentityResourceViewModel identityResourceViewModel = null);

        EditableUserViewModel<T> GetEditableUserViewModel<T>(IList<RoleViewModel> roles, T userViewModel = null, IList<string> validTwoFactorProviders = null)
            where T : UserViewModel, new();

        EditableApiResourceViewModel GetEditableApiResourceViewModel(IList<ClaimTypeViewModel> claimTypes, 
            ApiResourceViewModel apiResourceViewModel = null);

        EditableSecretViewModel GetEditableSecretForApiViewModel(int apiResourceId);

        EditableScopeViewModel GetEditableScopeViewModel(int apiResourceId, IList<ClaimTypeViewModel> claimTypes,
            ScopeViewModel scope = null);

        EditableClientViewModel GetEditableClientViewModel(IList<GrantTypeViewModel> grantTypes,
            IList<IdentityResourceViewModel> identityResources, IList<ScopeViewModel> scopes,
            ClientViewModel clientEntity = null);

        EditableSecretViewModel GetEditableSecretForClientViewModel(int clientId);

        EditableApiAccessKeyViewModel GetEditableApiAccessKeyViewModel(
            IList<SelectableViewModel<ApiAccessPermissionEnumViewModel>> selectableApiPermissions, ApiAccessKeyViewModel apiAccessKey = null);

        EditableApiAccessKeyHashViewModel GetEditableApiAccessKeyHashViewModel(
            IList<string> algorithms, ApiAccessKeyHashViewModel apiAccessKeyHash = null);
    }
}