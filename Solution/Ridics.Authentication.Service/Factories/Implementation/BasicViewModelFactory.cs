using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Factories.Implementation
{
    public class BasicViewModelFactory : IBasicViewModelFactory
    {
        private readonly IGenericViewModelFactory m_genericViewModelFactory;
        private readonly ExternalLoginProviderManager m_externalLoginProviderManager;
        private readonly IFileResourceManager m_fileResourceManager;
        private readonly IMapper m_mapper;

        public BasicViewModelFactory(
            IGenericViewModelFactory genericViewModelFactory,
            ExternalLoginProviderManager externalLoginProviderManager,
            IFileResourceManager fileResourceManager,
            IMapper mapper
        )
        {
            m_genericViewModelFactory = genericViewModelFactory;
            m_externalLoginProviderManager = externalLoginProviderManager;
            m_fileResourceManager = fileResourceManager;
            m_mapper = mapper;
        }

        public IList<ExternalLoginProviderViewModel> FindExternalProviderViewModels()
        {
            var externalLoginProviders = m_externalLoginProviderManager.FindAllExternalLoginProviders();

            var enabledExternalLoginProviders = externalLoginProviders.Result.Where(x => x.Enable && !x.HideOnLoginScreen).ToList();

            var externalLoginProviderList = m_mapper.Map<IList<ExternalLoginProviderViewModel>>(enabledExternalLoginProviders);

            foreach (var externalLoginProvider in externalLoginProviderList.ToList())
            {
                externalLoginProvider.LogoFileName = m_fileResourceManager.ResolveFullPath(externalLoginProvider.Logo);
            }

            return externalLoginProviderList;
        }

        public LoginViewModel GetLoginViewModel(string returnUrl)
        {
            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = FindExternalProviderViewModels(),
            };

            return loginViewModel;
        }

        public ClaimTypeViewModel GetClaimTypeViewModel(IList<ClaimTypeEnumViewModel> allTypes)
        {
            var vm = new ClaimTypeViewModel
            {
                AllTypeValues = allTypes
            };

            return vm;
        }

        public UserClaimsViewModel GetUserClaimsViewModel(IList<ClaimViewModel> userClaims, int userId,
            string dialogTitle,
            string dialogMessage)
        {
            var confirmDialog = m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            return new UserClaimsViewModel
            {
                UserId = userId,
                Items = userClaims,
                DeleteConfirmDialog = confirmDialog
            };
        }

        public ApiResourceSecretsViewModel GetApiResourceSecretsViewModel(IList<SecretViewModel> secrets,
            int apiResourceId,
            string dialogTitle, string dialogMessage)
        {
            var confirmDialog = m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            return new ApiResourceSecretsViewModel
            {
                ApiResourceId = apiResourceId,
                Items = secrets,
                DeleteConfirmDialog = confirmDialog
            };
        }

        public ApiResourceScopesViewModel GetApiResourceScopesViewModel(IList<ScopeViewModel> scopes, int apiResourceId,
            string dialogTitle,
            string dialogMessage)
        {
            var confirmDialog = m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            return new ApiResourceScopesViewModel
            {
                ApiResourceId = apiResourceId,
                Items = scopes,
                DeleteConfirmDialog = confirmDialog
            };
        }

        public ResourcePermissionTypeActionsViewModel GetPermissionTypeActionsViewModel(
            IList<ResourcePermissionTypeActionViewModel> actions,
            int resourcePermissionTypeId,
            string dialogTitle,
            string dialogMessage)
        {
            var confirmDialog = m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            return new ResourcePermissionTypeActionsViewModel
            {
                ResourcePermissionTypeId = resourcePermissionTypeId,
                Items = actions,
                DeleteConfirmDialog = confirmDialog
            };
        }

        public UriViewModel GetUriViewModel(int clientId)
        {
            return new UriViewModel
            {
                ClientId = clientId
            };
        }

        public ClientSecretsViewModel GetClientSecretsViewModel(IList<SecretViewModel> secrets, int clientId,
            string dialogTitle, string dialogMessage)
        {
            var confirmDialog = m_genericViewModelFactory.GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            return new ClientSecretsViewModel
            {
                ClientId = clientId,
                Items = secrets,
                DeleteConfirmDialog = confirmDialog
            };
        }

        public ConsentViewModel GetConsentViewModel(string returnUrl, ClientViewModel client,
            IList<ScopeViewModel> scopes,
            IList<IdentityResourceViewModel> identityResources)
        {
            var selectableIdentityResources = m_genericViewModelFactory.GetSelectableViewmodelList(identityResources);
            selectableIdentityResources.ForEach(x => x.IsSelected = x.Item.Required);

            var selectableScopes = m_genericViewModelFactory.GetSelectableViewmodelList(scopes);
            selectableScopes.ForEach(x => x.IsSelected = x.Item.Required);

            var viewModel = new ConsentViewModel
            {
                RememberConsent = true,
                ReturnUrl = returnUrl,
                Client = client,
                IdentityResources = selectableIdentityResources,
                Scopes = selectableScopes,
            };

            return viewModel;
        }
    }
}
