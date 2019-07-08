using System.Collections.Generic;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Keys;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Factories.Implementation
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IGenericViewModelFactory m_genericViewModelFactory;
        private readonly IBasicViewModelFactory m_basicViewModelFactory;
        private readonly IEditableViewModelFactory m_editableViewModelFactory;

        public ViewModelFactory(IGenericViewModelFactory genericViewModelFactory, IBasicViewModelFactory basicViewModelFactory,
            IEditableViewModelFactory editableViewModelFactory)
        {
            m_genericViewModelFactory = genericViewModelFactory;
            m_basicViewModelFactory = basicViewModelFactory;
            m_editableViewModelFactory = editableViewModelFactory;
        }

        public ConfirmDialogViewModel GetConfirmDialogViewmodel(string dialogTitle, string dialogMessage)
        {
            return m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);
        }
        
        public ConfirmDialogViewModel GetConfirmDialogViewmodel(string id, string dialogTitle, string dialogMessage)
        {
            return m_genericViewModelFactory.GetConfirmDialogViewmodel(id, dialogTitle, dialogMessage);
        }

        public ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount,
            int itemsOnPage)
        {
            return m_genericViewModelFactory.GetListViewModel(items, dialogTitle, dialogMessage, itemsCount, itemsOnPage);
        }

        public ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount)
        {
            return m_genericViewModelFactory.GetListViewModel(items, dialogTitle, dialogMessage, itemsCount);
        }

        public ViewModel<T> GetViewModel<T>(T item, string dialogTitle, string dialogMessage)
        {
            return m_genericViewModelFactory.GetViewModel(item, dialogTitle, dialogMessage);
        }

        public List<SelectableViewModel<T>> GetSelectableViewmodelList<T>(IEnumerable<T> viewModelList)
        {
            return m_genericViewModelFactory.GetSelectableViewmodelList(viewModelList);
        }

        public IList<ExternalLoginProviderViewModel> FindExternalProviderViewModels()
        {
            return m_basicViewModelFactory.FindExternalProviderViewModels();
        }

        public LoginViewModel GetLoginViewModel(string returnUrl)
        {
            return m_basicViewModelFactory.GetLoginViewModel(returnUrl);
        }

        public ClaimTypeViewModel GetClaimTypeViewModel(IList<ClaimTypeEnumViewModel> allTypes)
        {
            return m_basicViewModelFactory.GetClaimTypeViewModel(allTypes);
        }

        public UserClaimsViewModel GetUserClaimsViewModel(IList<ClaimViewModel> userClaims, int userId, string dialogTitle,
            string dialogMessage)
        {
            return m_basicViewModelFactory.GetUserClaimsViewModel(userClaims, userId, dialogTitle, dialogMessage);
        }

        public ApiResourceSecretsViewModel GetApiResourceSecretsViewModel(IList<SecretViewModel> secrets, int apiResourceId,
            string dialogTitle, string dialogMessage)
        {
            return m_basicViewModelFactory.GetApiResourceSecretsViewModel(secrets, apiResourceId, dialogTitle, dialogMessage);
        }

        public ApiResourceScopesViewModel GetApiResourceScopesViewModel(IList<ScopeViewModel> scopes, int apiResourceId, string dialogTitle,
            string dialogMessage)
        {
            return m_basicViewModelFactory.GetApiResourceScopesViewModel(scopes, apiResourceId, dialogTitle, dialogMessage);
        }

        public ResourcePermissionTypeActionsViewModel GetPermissionTypeActionsViewModel(
            IList<ResourcePermissionTypeActionViewModel> actions,
            int resourcePermissionTypeId,
            string dialogTitle,
            string dialogMessage)
        {
            return m_basicViewModelFactory.GetPermissionTypeActionsViewModel(actions, resourcePermissionTypeId, dialogTitle, dialogMessage);
        }

        public UriViewModel GetUriViewModel(int clientId)
        {
            return m_basicViewModelFactory.GetUriViewModel(clientId);
        }

        public ClientSecretsViewModel GetClientSecretsViewModel(IList<SecretViewModel> secrets, int clientId, string dialogTitle,
            string dialogMessage)
        {
            return m_basicViewModelFactory.GetClientSecretsViewModel(secrets, clientId, dialogTitle, dialogMessage);
        }

        public ConsentViewModel GetConsentViewModel(string returnUrl, ClientViewModel client, IList<ScopeViewModel> scopes,
            IList<IdentityResourceViewModel> identityResources)
        {
            return m_basicViewModelFactory.GetConsentViewModel(returnUrl, client, scopes, identityResources);
        }

        public EditableClaimViewModel GetAddClaimViewModel(int userId, IList<ClaimTypeViewModel> claimTypes)
        {
            return m_editableViewModelFactory.GetAddClaimViewModel(userId, claimTypes);
        }

        public EditableIdentityResourceViewModel GetEditableIdentityResourceViewModel(IList<ClaimTypeViewModel> claimTypes,
            IdentityResourceViewModel identityResourceViewModel = null)
        {
            return m_editableViewModelFactory.GetEditableIdentityResourceViewModel(claimTypes, identityResourceViewModel);
        }

        public EditableUserViewModel<T> GetEditableUserViewModel<T>(IList<RoleViewModel> roles, T userViewModel = default(T),
            IList<string> validTwoFactorProviders = null)
            where T : UserViewModel, new()
        {
            return m_editableViewModelFactory.GetEditableUserViewModel(roles, userViewModel, validTwoFactorProviders);
        }

        public EditableApiResourceViewModel GetEditableApiResourceViewModel(IList<ClaimTypeViewModel> claimTypes,
            ApiResourceViewModel apiResourceViewModel = null)
        {
            return m_editableViewModelFactory.GetEditableApiResourceViewModel(claimTypes, apiResourceViewModel);
        }

        public EditableSecretViewModel GetEditableSecretForApiViewModel(int apiResourceId)
        {
            return m_editableViewModelFactory.GetEditableSecretForApiViewModel(apiResourceId);
        }

        public EditableScopeViewModel GetEditableScopeViewModel(int apiResourceId, IList<ClaimTypeViewModel> claimTypes,
            ScopeViewModel scope = null)
        {
            return m_editableViewModelFactory.GetEditableScopeViewModel(apiResourceId, claimTypes, scope);
        }

        public EditableClientViewModel GetEditableClientViewModel(IList<GrantTypeViewModel> grantTypes, IList<IdentityResourceViewModel> identityResources, IList<ScopeViewModel> scopes,
            ClientViewModel clientEntity = null)
        {
            return m_editableViewModelFactory.GetEditableClientViewModel(grantTypes, identityResources, scopes, clientEntity);
        }

        public EditableSecretViewModel GetEditableSecretForClientViewModel(int clientId)
        {
            return m_editableViewModelFactory.GetEditableSecretForClientViewModel(clientId);
        }

        public EditableApiAccessKeyViewModel GetEditableApiAccessKeyViewModel(IList<SelectableViewModel<ApiAccessPermissionEnumViewModel>> selectableApiPermissions, ApiAccessKeyViewModel apiAccessKey = null)
        {
            return m_editableViewModelFactory.GetEditableApiAccessKeyViewModel(selectableApiPermissions, apiAccessKey);
        }

        public EditableApiAccessKeyHashViewModel GetEditableApiAccessKeyHashViewModel(IList<string> algorithms, ApiAccessKeyHashViewModel apiAccessKeyHash = null)
        {
            return m_editableViewModelFactory.GetEditableApiAccessKeyHashViewModel(algorithms, apiAccessKeyHash);
        }
    }
}