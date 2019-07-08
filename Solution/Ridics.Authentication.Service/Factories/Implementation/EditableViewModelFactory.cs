using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Keys;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Factories.Implementation
{
    public class EditableViewModelFactory : IEditableViewModelFactory
    {
        private readonly IGenericViewModelFactory m_genericViewModelFactory;
        private readonly ExternalLoginProviderManager m_externalLoginProviderManager;
        private readonly IFileResourceManager m_fileResourceManager;
        private readonly IMapper m_mapper;


        public EditableViewModelFactory(
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

        public EditableClaimViewModel GetAddClaimViewModel(int userId, IList<ClaimTypeViewModel> claimTypes)
        {
            return new EditableClaimViewModel
            {
                UserId = userId,
                ClaimTypes = claimTypes
            };
        }

        public EditableIdentityResourceViewModel GetEditableIdentityResourceViewModel(
            IList<ClaimTypeViewModel> claimTypes, IdentityResourceViewModel identityResourceViewModel = null)
        {
            var selectableClaimTypes = m_genericViewModelFactory.GetSelectableViewmodelList(claimTypes);

            if (identityResourceViewModel != null)
                selectableClaimTypes.ForEach(claim =>
                    claim.IsSelected =
                        identityResourceViewModel.Claims?.FirstOrDefault(userClaim => userClaim.Id == claim.Item.Id) !=
                        null);

            return new EditableIdentityResourceViewModel
            {
                IdentityResourceViewModel = identityResourceViewModel ?? new IdentityResourceViewModel(),
                SelectableClaimTypes = selectableClaimTypes
            };
        }


        public EditableUserViewModel<T> GetEditableUserViewModel<T>(IList<RoleViewModel> roles, T userViewModel = null,
            IList<string> validTwoFactorProviders = null)
            where T : UserViewModel, new()
        {
            var selectableRoles = m_genericViewModelFactory.GetSelectableViewmodelList(roles);

            var enabledExternalLoginProviders = m_externalLoginProviderManager.FindManageableExternalLoginProviders()
                .Result.Where(x => x.Enable).ToList();

            var availableExternalLoginProviders = m_mapper.Map<IList<ExternalLoginProviderViewModel>>(
                enabledExternalLoginProviders
            );

            foreach (var externalLoginProvider in availableExternalLoginProviders)
            {
                externalLoginProvider.LogoFileName = m_fileResourceManager.ResolveFullPath(externalLoginProvider.Logo);

                if (userViewModel != null)
                {
                    foreach (var externalUserLogin in userViewModel.ExternalUserLogins.Where(x =>
                        Equals(x.LoginProvider, externalLoginProvider))
                    )
                    {
                        externalUserLogin.LoginProvider.LogoFileName = externalLoginProvider.LogoFileName;
                    }
                }
            }

            if (userViewModel != null)
            {
                selectableRoles.ForEach(role =>
                    role.IsSelected = userViewModel.Roles?
                                          .FirstOrDefault(userRole => userRole.Id == role.Item.Id)
                                      != null
                );

                if (validTwoFactorProviders != null)
                {
                    userViewModel.TwoFactorProviders = validTwoFactorProviders;
                }

                foreach (var externalLogin in userViewModel.ExternalUserLogins.ToList())
                {
                    if (!externalLogin.LoginProvider.Enable)
                    {
                        userViewModel.ExternalUserLogins.Remove(externalLogin);
                    }

                    availableExternalLoginProviders.Remove(externalLogin.LoginProvider);

                    if (string.IsNullOrEmpty(externalLogin.LoginProvider.LogoFileName))
                    {
                        externalLogin.LoginProvider.LogoFileName = m_fileResourceManager.ResolveFullPath(externalLogin.LoginProvider.Logo);
                    }
                }
            }

            return new EditableUserViewModel<T>
            {
                UserViewModel = userViewModel ?? new T(),

                SelectableRoles = selectableRoles,
                AvailableExternalLoginProviders = availableExternalLoginProviders
            };
        }

        public EditableApiResourceViewModel GetEditableApiResourceViewModel(IList<ClaimTypeViewModel> claimTypes,
            ApiResourceViewModel apiResourceViewModel = null)
        {
            var selectableClaimTypes = m_genericViewModelFactory.GetSelectableViewmodelList(claimTypes);

            if (apiResourceViewModel != null)
            {
                selectableClaimTypes.ForEach(claim =>
                    claim.IsSelected =
                        apiResourceViewModel.Claims?.FirstOrDefault(userClaim => userClaim.Id == claim.Item.Id) !=
                        null);
            }

            return new EditableApiResourceViewModel
            {
                ApiResourceViewModel = apiResourceViewModel ?? new ApiResourceViewModel(),
                SelectableClaimTypes = selectableClaimTypes,
            };
        }

        public EditableSecretViewModel GetEditableSecretForApiViewModel(int apiResourceId)
        {
            return new EditableSecretViewModel
            {
                ApiResourceId = apiResourceId
            };
        }


        public EditableScopeViewModel GetEditableScopeViewModel(int apiResourceId, IList<ClaimTypeViewModel> claimTypes,
            ScopeViewModel scope = null)
        {
            var selectableClaimTypes = m_genericViewModelFactory.GetSelectableViewmodelList(claimTypes);

            if (scope != null)
                selectableClaimTypes.ForEach(claim =>
                    claim.IsSelected =
                        scope.Claims?.FirstOrDefault(userClaim => userClaim.Id == claim.Item.Id) != null);

            return new EditableScopeViewModel
            {
                ApiResourceId = apiResourceId,
                SelectableClaimTypes = selectableClaimTypes
            };
        }


        public EditableClientViewModel GetEditableClientViewModel(IList<GrantTypeViewModel> grantTypes,
            IList<IdentityResourceViewModel> identityResources, IList<ScopeViewModel> scopes, ClientViewModel clientEntity = null)
        {
            var selectableGrantTypes = m_genericViewModelFactory.GetSelectableViewmodelList(grantTypes);
            var selectableIdentityResources = m_genericViewModelFactory.GetSelectableViewmodelList(identityResources);
            var selectableScopes = m_genericViewModelFactory.GetSelectableViewmodelList(scopes);

            var editableClient = new EditableClientViewModel();

            if (clientEntity != null)
            {
                editableClient.Id = clientEntity.Id;
                editableClient.Name = clientEntity.Name;
                editableClient.Description = clientEntity.Description;
                editableClient.Secrets = clientEntity.Secrets;
                editableClient.AllowedGrantTypes = clientEntity.AllowedGrantTypes;
                editableClient.UriList = clientEntity.UriList;
                editableClient.AllowedIdentityResources = clientEntity.AllowedIdentityResources;
                editableClient.DisplayUrl = clientEntity.DisplayUrl;
                editableClient.LogoUrl = clientEntity.LogoUrl;
                editableClient.RequireConsent = clientEntity.RequireConsent;
                editableClient.AllowAccessTokensViaBrowser = clientEntity.AllowAccessTokensViaBrowser;

                selectableGrantTypes.ForEach(x =>
                    x.IsSelected = clientEntity.AllowedGrantTypes?.FirstOrDefault(y => y.Id == x.Item.Id) != null);
                selectableIdentityResources.ForEach(x =>
                    x.IsSelected = clientEntity.AllowedIdentityResources?.FirstOrDefault(y => y.Id == x.Item.Id) !=
                                   null);
                selectableScopes.ForEach(x =>
                    x.IsSelected = clientEntity.AllowedScopes?.FirstOrDefault(y => y.Id == x.Item.Id) != null);
            }

            editableClient.SelectableGrantTypes = selectableGrantTypes;
            editableClient.SelectableIdentityresources = selectableIdentityResources;
            editableClient.SelectableScopes = selectableScopes;

            return editableClient;
        }

        public EditableSecretViewModel GetEditableSecretForClientViewModel(int clientId)
        {
            return new EditableSecretViewModel
            {
                ClientId = clientId
            };
        }

        public EditableApiAccessKeyHashViewModel GetEditableApiAccessKeyHashViewModel(IList<string> algorithms, ApiAccessKeyHashViewModel apiAccessKeyHash = null)
        {
            var editableApiAccessKeyHash = new EditableApiAccessKeyHashViewModel();

            if (apiAccessKeyHash != null)
            {
                editableApiAccessKeyHash.Name = apiAccessKeyHash.Name;
                editableApiAccessKeyHash.Algorithm = apiAccessKeyHash.Algorithm;
            }

            var selectAlgorithmList = algorithms.Select(x => new SelectListItem(x, x, x.Equals(editableApiAccessKeyHash.Algorithm, StringComparison.InvariantCultureIgnoreCase))).ToList();
            editableApiAccessKeyHash.SelectableAlgorithms = selectAlgorithmList;

            return editableApiAccessKeyHash;
        }

        public EditableApiAccessKeyViewModel GetEditableApiAccessKeyViewModel(
            IList<SelectableViewModel<ApiAccessPermissionEnumViewModel>> selectableApiPermissions,
            ApiAccessKeyViewModel apiAccessKey = null)
        {
            var editableApiAccessKey = new EditableApiAccessKeyViewModel();
            editableApiAccessKey.EditableApiAccessKeyHashViewModel = new EditableApiAccessKeyHashViewModel();

            if (apiAccessKey != null)
            {
                editableApiAccessKey.Id = apiAccessKey.Id;
                editableApiAccessKey.Name = apiAccessKey.Name;
                editableApiAccessKey.Permissions = apiAccessKey.Permissions;

                if (apiAccessKey.Permissions != null)
                {
                    foreach (var selectableApiPermission in selectableApiPermissions)
                    {
                        foreach (var apiAccessPermission in apiAccessKey.Permissions)
                        {
                            if (selectableApiPermission.Item == apiAccessPermission)
                            {
                                selectableApiPermission.IsSelected = true;
                                break;
                            }
                        }
                    }
                }

                if (apiAccessKey.ApiAccessKeyHashViewModel != null)
                {
                    editableApiAccessKey.EditableApiAccessKeyHashViewModel.Algorithm = apiAccessKey.ApiAccessKeyHashViewModel.Algorithm;
                }
            }
            editableApiAccessKey.SelectableApiPermissions = selectableApiPermissions;
            return editableApiAccessKey;
        }
    }
}
