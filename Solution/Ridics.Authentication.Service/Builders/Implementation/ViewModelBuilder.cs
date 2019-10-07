using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Builders.Interface;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.DynamicModule;
using Ridics.Authentication.Service.Models.ViewModel.Keys;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.Builders.Implementation
{
    public class ViewModelBuilder : IViewModelBuilder
    {
        private readonly IViewModelFactory m_viewModelFactory;
        private readonly UserManager m_userManager;
        private readonly RoleManager m_roleManager;
        private readonly ClaimTypeManager m_claimTypeManager;
        private readonly PermissionManager m_permissionManager;
        private readonly ResourcePermissionManager m_resourcePermissionManager;
        private readonly ResourcePermissionTypeManager m_resourcePermissionTypeManager;
        private readonly ResourcePermissionTypeActionManager m_resourcePermissionTypeActionManager;
        private readonly ApiResourceManager m_apiResourceManager;
        private readonly ClientManager m_clientManager;
        private readonly IdentityResourceManager m_identityResourceManager;
        private readonly GrantTypeManager m_grantTypeManager;
        private readonly IdentityUserManager m_identityUserManager;
        private readonly DynamicModuleManager m_dynamicModuleManager;
        private readonly DynamicModuleProvider m_dynamicModuleProvider;
        private readonly IFileResourceManager m_fileResourceManager;
        private readonly IMapper m_mapper;
        private readonly IModuleLocalization m_moduleLocalization;
        private readonly HashManager m_hashManager;
        private readonly ScopeManager m_scopesManager;

        public ViewModelBuilder(
            IViewModelFactory viewModelFactory,
            UserManager userManager,
            RoleManager roleManager,
            ClaimTypeManager claimTypeManager,
            PermissionManager permissionManager,
            ResourcePermissionManager resourcePermissionManager,
            ApiResourceManager apiResourceManager,
            ResourcePermissionTypeActionManager resourcePermissionTypeActionManager,
            ClientManager clientManager,
            IdentityResourceManager identityResourceManager,
            GrantTypeManager grantTypeManager,
            IdentityUserManager identityUserManager,
            ResourcePermissionTypeManager resourcePermissionTypeManager,
            DynamicModuleManager dynamicModuleManager,
            DynamicModuleProvider dynamicModuleProvider,
            IFileResourceManager fileResourceManager,
            IMapper mapper,
            IModuleLocalization moduleLocalization, 
            HashManager hashManager, 
            ScopeManager scopesManager)
        {
            m_viewModelFactory = viewModelFactory;
            m_userManager = userManager;
            m_roleManager = roleManager;
            m_claimTypeManager = claimTypeManager;
            m_permissionManager = permissionManager;
            m_resourcePermissionManager = resourcePermissionManager;
            m_resourcePermissionTypeActionManager = resourcePermissionTypeActionManager;
            m_apiResourceManager = apiResourceManager;
            m_clientManager = clientManager;
            m_identityResourceManager = identityResourceManager;
            m_grantTypeManager = grantTypeManager;
            m_identityUserManager = identityUserManager;
            m_resourcePermissionTypeManager = resourcePermissionTypeManager;
            m_dynamicModuleManager = dynamicModuleManager;
            m_dynamicModuleProvider = dynamicModuleProvider;
            m_fileResourceManager = fileResourceManager;
            m_mapper = mapper;
            m_moduleLocalization = moduleLocalization;
            this.m_hashManager = hashManager;
            m_scopesManager = scopesManager;
        }

        public List<SelectableViewModel<RoleViewModel>> BuildUserRolesViewModel(ModelStateDictionary modelState, int id)
        {
            var userResult = m_userManager.GetUserById(id);
            var rolesResult = m_roleManager.GetAllRoles();

            if (userResult.HasError)
            {
                modelState.AddModelError(userResult.Error.Message);
                return null;
            }

            if (rolesResult.HasError)
            {
                modelState.AddModelError(rolesResult.Error.Message);
                return null;
            }

            var userRoles = m_mapper.Map<IList<RoleViewModel>>(userResult.Result.Roles);
            var roleViewModels = m_mapper.Map<IList<RoleViewModel>>(rolesResult.Result);

            var selectableRoles = m_viewModelFactory.GetSelectableViewmodelList(roleViewModels);

            selectableRoles.ForEach(role => role.IsSelected = userRoles.FirstOrDefault(userRole => userRole.Id == role.Item.Id) != null);

            return selectableRoles;
        }

        public EditableClaimViewModel BuildAddClaimViewModel(ModelStateDictionary modelState, int id)
        {
            var claimTypesResult = m_claimTypeManager.GetAllClaimTypes();

            if (claimTypesResult.HasError)
            {
                modelState.AddModelError(claimTypesResult.Error.Message);
                return null;
            }

            var claimTypeViewModels = m_mapper.Map<IList<ClaimTypeViewModel>>(claimTypesResult.Result);

            var viewModel = m_viewModelFactory.GetAddClaimViewModel(id, claimTypeViewModels);

            return viewModel;
        }

        public EditableUserViewModel<UserWithPasswordViewModel> BuildCreateUserViewModel(ModelStateDictionary modelState,
            UserWithPasswordViewModel viewModel = null)
        {
            var allRolesResult = m_roleManager.GetAllRoles();

            if (allRolesResult.HasError)
            {
                modelState.AddModelError(allRolesResult.Error.Message);
            }

            var roleViewModels = m_mapper.Map<IList<RoleViewModel>>(allRolesResult.Result);

            return viewModel == null
                ? m_viewModelFactory.GetEditableUserViewModel<UserWithPasswordViewModel>(roleViewModels)
                : m_viewModelFactory.GetEditableUserViewModel(roleViewModels, viewModel);
        }

        public async Task<EditableUserViewModel<UserViewModel>> BuildEditableUserViewModelAsync(ModelStateDictionary modelState,
            UserViewModel viewModel = null)
        {
            var allRolesResult = m_roleManager.GetAllRoles();

            if (allRolesResult.HasError)
            {
                modelState.AddModelError(allRolesResult.Error.Message);
            }

            var roleViewModels = m_mapper.Map<IList<RoleViewModel>>(allRolesResult.Result);

            IList<string> validTwoFactorProviders = null;

            if (viewModel != null)
            {
                var applicationUser = await m_identityUserManager.GetUserByIdAsync(viewModel.Id);

                validTwoFactorProviders = await m_identityUserManager.GetValidTwoFactorProvidersAsync(applicationUser);
            }

            return viewModel == null
                ? m_viewModelFactory.GetEditableUserViewModel<UserViewModel>(roleViewModels)
                : m_viewModelFactory.GetEditableUserViewModel(roleViewModels, viewModel, validTwoFactorProviders);
        }


        public List<SelectableViewModel<PermissionViewModel>> BuildPermissionsViewModel(ModelStateDictionary modelState, int id)
        {
            var roleResult = m_roleManager.FindRoleById(id);

            var permissionsResult = m_permissionManager.GetAllPermissions();

            if (roleResult.HasError)
            {
                modelState.AddModelError(roleResult.Error.Message);
                return null;
            }

            if (permissionsResult.HasError)
            {
                modelState.AddModelError(permissionsResult.Error.Message);
                return null;
            }

            var rolePermissionViewModels = m_mapper.Map<IList<PermissionViewModel>>(roleResult.Result.Permissions);
            var permissionViewModels = m_mapper.Map<IList<PermissionViewModel>>(permissionsResult.Result);

            var selectablePermissions = m_viewModelFactory.GetSelectableViewmodelList(permissionViewModels);

            selectablePermissions.ForEach(permission =>
                permission.IsSelected =
                    rolePermissionViewModels.FirstOrDefault(rolePermission => rolePermission.Id == permission.Item.Id) != null);

            return selectablePermissions;
        }

        public List<SelectableViewModel<ResourcePermissionViewModel>> BuildRoleResourcePermissionsViewModel(ModelStateDictionary modelState,
            int id)
        {
            var roleResult = m_roleManager.FindRoleById(id);

            var permissionsResult = m_resourcePermissionManager.GetAllPermissions();

            if (roleResult.HasError)
            {
                modelState.AddModelError(roleResult.Error.Message);
                return null;
            }

            if (permissionsResult.HasError)
            {
                modelState.AddModelError(permissionsResult.Error.Message);
                return null;
            }

            var rolePermissionViewModels = Mapper.Map<IList<ResourcePermissionViewModel>>(roleResult.Result.ResourcePermissions);
            var permissionViewModels = Mapper.Map<IList<ResourcePermissionViewModel>>(permissionsResult.Result);

            var selectablePermissions = m_viewModelFactory.GetSelectableViewmodelList(permissionViewModels);

            selectablePermissions.ForEach(permission =>
                permission.IsSelected =
                    rolePermissionViewModels.FirstOrDefault(rolePermission => rolePermission.Id == permission.Item.Id) != null);

            return selectablePermissions;
        }

        public List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildRoleResourcePermissionTypeActionsViewModel(
            ModelStateDictionary modelState, int id)
        {
            var roleResult = m_roleManager.FindRoleById(id);

            var permissionsResult = m_resourcePermissionTypeActionManager.GetAllPermissionTypeActions();

            if (roleResult.HasError)
            {
                modelState.AddModelError(roleResult.Error.Message);
                return null;
            }

            if (permissionsResult.HasError)
            {
                modelState.AddModelError(permissionsResult.Error.Message);
                return null;
            }

            var rolePermissionViewModels =
                Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(roleResult.Result.ResourcePermissionTypeActions);
            var permissionViewModels = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(permissionsResult.Result);

            var selectablePermissions = m_viewModelFactory.GetSelectableViewmodelList(permissionViewModels);

            selectablePermissions.ForEach(permission =>
                permission.IsSelected =
                    rolePermissionViewModels.FirstOrDefault(rolePermission => rolePermission.Id == permission.Item.Id) != null);

            return selectablePermissions;
        }

        public List<SelectableViewModel<ResourcePermissionViewModel>> BuildUserResourcePermissionsViewModel(ModelStateDictionary modelState,
            int id)
        {
            var userResult = m_userManager.GetUserById(id);

            var permissionsResult = m_resourcePermissionManager.GetAllPermissions();

            if (userResult.HasError)
            {
                modelState.AddModelError(userResult.Error.Message);
                return null;
            }

            if (permissionsResult.HasError)
            {
                modelState.AddModelError(permissionsResult.Error.Message);
                return null;
            }

            var userPermissionViewModels = Mapper.Map<IList<ResourcePermissionViewModel>>(userResult.Result.ResourcePermissions);
            var permissionViewModels = Mapper.Map<IList<ResourcePermissionViewModel>>(permissionsResult.Result);

            var selectablePermissions = m_viewModelFactory.GetSelectableViewmodelList(permissionViewModels);

            selectablePermissions.ForEach(permission =>
                permission.IsSelected =
                    userPermissionViewModels.FirstOrDefault(userPermission => userPermission.Id == permission.Item.Id) != null);

            return selectablePermissions;
        }

        public List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildUserResourcePermissionTypeActionsViewModel(
            ModelStateDictionary modelState, int id)
        {
            var userResult = m_userManager.GetUserById(id);

            var permissionsResult = m_resourcePermissionTypeActionManager.GetAllPermissionTypeActions();

            if (userResult.HasError)
            {
                modelState.AddModelError(userResult.Error.Message);
                return null;
            }

            if (permissionsResult.HasError)
            {
                modelState.AddModelError(permissionsResult.Error.Message);
                return null;
            }

            var userPermissionViewModels =
                Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(userResult.Result.ResourcePermissionTypeActions);
            var permissionViewModels = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(permissionsResult.Result);

            var selectablePermissions = m_viewModelFactory.GetSelectableViewmodelList(permissionViewModels);

            selectablePermissions.ForEach(permission =>
                permission.IsSelected =
                    userPermissionViewModels.FirstOrDefault(userPermission => userPermission.Id == permission.Item.Id) != null);

            return selectablePermissions;
        }


        public IList<SelectableViewModel<RoleViewModel>> BuildPermissionRolesViewModel(ModelStateDictionary modelState, int permissionId)
        {
            var permissionResult = m_permissionManager.FindPermissionById(permissionId, true);

            var rolesResult = m_roleManager.GetAllRoles();

            if (permissionResult.HasError)
            {
                modelState.AddModelError(permissionResult.Error.Message);
                return null;
            }

            if (rolesResult.HasError)
            {
                modelState.AddModelError(rolesResult.Error.Message);
                return null;
            }

            var roleViewModels = Mapper.Map<IList<RoleViewModel>>(rolesResult.Result);

            var permissionRoles = Mapper.Map<IList<RoleViewModel>>(permissionResult.Result.Roles);
            var selectableRoles = m_viewModelFactory.GetSelectableViewmodelList(roleViewModels);

            selectableRoles.ForEach(role =>
                role.IsSelected = permissionRoles.FirstOrDefault(permissionRole => permissionRole.Id == role.Item.Id) != null);

            return selectableRoles;
        }

        public IList<SelectableViewModel<RoleViewModel>> BuildResourcePermissionRolesViewModel(ModelStateDictionary modelState,
            int resourcePermissionId)
        {
            var permissionResult = m_resourcePermissionManager.FindPermissionById(resourcePermissionId);

            var rolesResult = m_roleManager.GetAllRoles();

            if (permissionResult.HasError)
            {
                modelState.AddModelError(permissionResult.Error.Message);
                return null;
            }

            if (rolesResult.HasError)
            {
                modelState.AddModelError(rolesResult.Error.Message);
                return null;
            }

            var roleViewModels = Mapper.Map<IList<RoleViewModel>>(rolesResult.Result);

            var permissionRoles = Mapper.Map<IList<RoleViewModel>>(permissionResult.Result.Roles);
            var selectableRoles = m_viewModelFactory.GetSelectableViewmodelList(roleViewModels);

            selectableRoles.ForEach(role =>
                role.IsSelected = permissionRoles.FirstOrDefault(permissionRole => permissionRole.Id == role.Item.Id) != null);

            return selectableRoles;
        }

        public IList<SelectableViewModel<RoleViewModel>> BuildResourcePermissionTypeActionRolesViewModel(ModelStateDictionary modelState,
            int resourcePermissionTypeActionId)
        {
            var permissionResult = m_resourcePermissionTypeActionManager.FindPermissionTypeActionById(resourcePermissionTypeActionId);

            var rolesResult = m_roleManager.GetAllRoles();

            if (permissionResult.HasError)
            {
                modelState.AddModelError(permissionResult.Error.Message);
                return null;
            }

            if (rolesResult.HasError)
            {
                modelState.AddModelError(rolesResult.Error.Message);
                return null;
            }

            var roleViewModels = m_mapper.Map<IList<RoleViewModel>>(rolesResult.Result);

            var permissionRoles = m_mapper.Map<IList<RoleViewModel>>(permissionResult.Result.Roles);
            var selectableRoles = m_viewModelFactory.GetSelectableViewmodelList(roleViewModels);

            selectableRoles.ForEach(role =>
                role.IsSelected = permissionRoles.FirstOrDefault(permissionRole => permissionRole.Id == role.Item.Id) != null);

            return selectableRoles;
        }

        public IList<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildResourcePermissionTypeActionsViewModel(
            ModelStateDictionary modelState,
            int resourcePermissionTypeId)
        {
            var permissionResult = m_resourcePermissionTypeManager.FindPermissionTypeById(resourcePermissionTypeId);

            //HACK get only actions available for permission type
            var actionsResult = m_resourcePermissionTypeActionManager.GetAllPermissionTypeActions();

            if (permissionResult.HasError)
            {
                modelState.AddModelError(permissionResult.Error.Message);
                return null;
            }

            if (actionsResult.HasError)
            {
                modelState.AddModelError(actionsResult.Error.Message);
                return null;
            }

            var actionViewModels = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(actionsResult.Result);

            var permissionActions =
                Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(permissionResult.Result.ResourcePermissionTypeActions);
            var selectableActions = m_viewModelFactory.GetSelectableViewmodelList(actionViewModels);

            selectableActions.ForEach(action =>
                action.IsSelected = permissionActions.FirstOrDefault(permissionRole => permissionRole.Id == action.Item.Id) != null);

            return selectableActions;
        }

        public EditableIdentityResourceViewModel BuildEditableIdentityResourceViewModel(ModelStateDictionary modelState,
            IdentityResourceViewModel viewModel = null)
        {
            var claimTypesResult = m_claimTypeManager.GetAllClaimTypes();

            if (claimTypesResult.HasError)
            {
                modelState.AddModelError(claimTypesResult.Error.Message);
            }

            var claimTypeViewModels = m_mapper.Map<IList<ClaimTypeViewModel>>(claimTypesResult.Result);

            return m_viewModelFactory.GetEditableIdentityResourceViewModel(claimTypeViewModels, viewModel);
        }

        public ConsentViewModel BuildConsentViewModel(ModelStateDictionary modelState, AuthorizationRequest request, string returnUrl)
        {
            var clientResult = m_clientManager.FindClientByClientId(request.ClientId);

            if (clientResult.HasError)
            {
                modelState.AddModelError(clientResult.Error.Message);
            }

            var scopesResult = m_scopesManager.GetAllScopes();

            if (scopesResult.HasError)
            {
                modelState.AddModelError(scopesResult.Error.Message);
            }

            var identityResourcesResult = m_identityResourceManager.GetAllIdentityResources();

            if (identityResourcesResult.HasError)
            {
                modelState.AddModelError(identityResourcesResult.Error.Message);
            }

            var scopes = scopesResult.Result?
                .Where(x => request.ScopesRequested.FirstOrDefault(y => y == x.Name) != null).ToList();

            var identityResources = identityResourcesResult.Result?
                .Where(x => request.ScopesRequested.FirstOrDefault(y => y == x.Name) != null).ToList();

            var clientViewModel = m_mapper.Map<ClientViewModel>(clientResult.Result);
            var scopeViewModels = m_mapper.Map<IList<ScopeViewModel>>(scopes);
            var identityResourceViewModels = m_mapper.Map<IList<IdentityResourceViewModel>>(identityResources);

            return m_viewModelFactory.GetConsentViewModel(returnUrl, clientViewModel, scopeViewModels, identityResourceViewModels);
        }

        public EditableSecretViewModel BuildClientAddSecretViewModel(ModelStateDictionary modelState, int clientId,
            EditableSecretViewModel secret = null)
        {
            if (secret == null)
            {
                return m_viewModelFactory.GetEditableSecretForClientViewModel(clientId);
            }

            secret.ClientId = clientId;
            return secret;
        }

        public EditableClientViewModel BuildEditableClientViewModel(ModelStateDictionary modelState, ClientViewModel viewModel = null)
        {
            var allGrantTypesResult = m_grantTypeManager.GetAllGrantTypes();

            if (allGrantTypesResult.HasError)
            {
                modelState.AddModelError(allGrantTypesResult.Error.Message);
            }

            var identityResourcesResult = m_identityResourceManager.GetAllIdentityResources();

            if (identityResourcesResult.HasError)
            {
                modelState.AddModelError(identityResourcesResult.Error.Message);
            }

            var scopesResult = m_scopesManager.GetAllScopes();

            if (scopesResult.HasError)
            {
                modelState.AddModelError(scopesResult.Error.Message);
            }

            var grantTypeViewModels = m_mapper.Map<IList<GrantTypeViewModel>>(allGrantTypesResult.Result);
            var identityResourceViewModels = m_mapper.Map<IList<IdentityResourceViewModel>>(identityResourcesResult.Result);
            var scopeViewModels = m_mapper.Map<IList<ScopeViewModel>>(scopesResult.Result);

            return m_viewModelFactory.GetEditableClientViewModel(grantTypeViewModels, identityResourceViewModels, scopeViewModels,
                viewModel);
        }

        public UriViewModel BuildCreateUriViewModel(ModelStateDictionary modelState, int clientId, UriViewModel uri = null)
        {
            if (uri == null)
            {
                return m_viewModelFactory.GetUriViewModel(clientId);
            }

            uri.ClientId = clientId;
            return uri;
        }


        public ClaimTypeViewModel BuildClaimTypeViewModel(ModelStateDictionary modelState, ClaimTypeViewModel claimTypeViewModel = null)
        {
            var claimTypesEnums = m_claimTypeManager.GetAllClaimTypeEnums();

            if (claimTypesEnums.HasError)
            {
                modelState.AddModelError(claimTypesEnums.Error.Message);
            }

            var claimTypeEnumViewModels = m_mapper.Map<IList<ClaimTypeEnumViewModel>>(claimTypesEnums.Result);

            if (claimTypeViewModel == null)
            {
                return m_viewModelFactory.GetClaimTypeViewModel(claimTypeEnumViewModels);
            }

            claimTypeViewModel.AllTypeValues = claimTypeEnumViewModels;

            return claimTypeViewModel;
        }

        public EditableApiResourceViewModel BuildEditableApiResourceViewModel(ModelStateDictionary modelState,
            ApiResourceViewModel viewModel = null)
        {
            var claimTypesResult = m_claimTypeManager.GetAllClaimTypes();

            if (claimTypesResult.HasError)
            {
                modelState.AddModelError(claimTypesResult.Error.Message);
            }

            var claimTypeViewModels = m_mapper.Map<IList<ClaimTypeViewModel>>(claimTypesResult.Result);

            return m_viewModelFactory.GetEditableApiResourceViewModel(claimTypeViewModels, viewModel);
        }


        public EditableSecretViewModel BuildAddSecretViewModel(ModelStateDictionary modelState, int apiResourceId,
            EditableSecretViewModel secret = null)
        {
            if (secret == null)
            {
                return m_viewModelFactory.GetEditableSecretForApiViewModel(apiResourceId);
            }

            secret.ApiResourceId = apiResourceId;
            return secret;
        }

        public EditableScopeViewModel BuildAddScopeViewModel(ModelStateDictionary modelState, int apiResourceId,
            EditableScopeViewModel scope = null)
        {
            var claimTypesResult = m_claimTypeManager.GetAllClaimTypes();

            if (claimTypesResult.HasError)
            {
                modelState.AddModelError(claimTypesResult.Error.Message);
            }

            var claimTypeViewModels = m_mapper.Map<IList<ClaimTypeViewModel>>(claimTypesResult.Result);

            return m_viewModelFactory.GetEditableScopeViewModel(apiResourceId, claimTypeViewModels, scope);
        }

        public CreateDynamicModuleViewModel BuildCreateDynamicModuleViewModel(
            ModelStateDictionary modelState,
            CreateDynamicModuleViewModel viewModel = null
        )
        {
            var availableDynamicModule = new List<string>();
            if (viewModel != null)
            {
                availableDynamicModule = m_dynamicModuleProvider.ModuleContexts
                    .Where(x =>
                        x.LibModuleInfo.ModuleGuid == viewModel.ModuleGuid
                        && x.DefaultModuleConfiguration?.Name != null
                    )
                    .Select(x => x.DefaultModuleConfiguration.Name).ToList();
            }

            return new CreateDynamicModuleViewModel
            {
                DynamicModules = m_dynamicModuleProvider.GetLibraryModuleInfos()
                    .GroupBy(x => x.ModuleGuid).Select(o => o.FirstOrDefault()).ToList(),
                ModuleGuid = viewModel?.ModuleGuid ?? default,
                AvailableDynamicModule = availableDynamicModule
            };
        }

        public DynamicModuleViewModel BuildEditDynamicModuleViewModel(
            ModelStateDictionary modelState,
            int dynamicModuleId,
            IDynamicModuleViewModel viewModel = null
        )
        {
            var dynamicModuleResult = m_dynamicModuleManager.GetById(dynamicModuleId);

            var dynamicModule = dynamicModuleResult.Result;

            var dynamicModuleViewModel = m_mapper.Map<DynamicModuleViewModel>(dynamicModule);

            var moduleInfo = m_dynamicModuleProvider.ModuleContexts
                .FirstOrDefault(x => x.LibModuleInfo.ModuleGuid == dynamicModuleViewModel.ModuleGuid);

            dynamicModuleViewModel.ModuleName = moduleInfo?.LibModuleInfo.DefaultDisplayName;

            var dynamicModuleContext = m_dynamicModuleProvider.GetContextByNameOrGuid(
                dynamicModuleViewModel.Name,
                dynamicModuleViewModel.ModuleGuid
            );

            if (moduleInfo != null)
            {
                var moduleConfiguration = dynamicModuleContext.ModuleConfiguration;

                if (moduleConfiguration == null && !string.IsNullOrEmpty(dynamicModule.ConfigurationString))
                {
                    moduleConfiguration = dynamicModule.Configuration(moduleInfo.ModuleConfigType);
                }

                if (moduleConfiguration == null)
                {
                    moduleConfiguration = dynamicModuleContext.EmptyModuleConfiguration;
                }

                dynamicModuleViewModel.CustomConfigurationPartialName = dynamicModuleContext.LibModuleInfo.ConfigureComponentName;
                dynamicModuleViewModel.CustomConfigurationViewModel = dynamicModuleContext.ModuleConfigurationManager.CreateViewModel(
                    moduleConfiguration,
                    m_moduleLocalization
                );

                if (dynamicModuleViewModel.CustomConfigurationViewModel.DisplayName == null)
                {
                    dynamicModuleViewModel.CustomConfigurationViewModel.DisplayName = moduleInfo?.LibModuleInfo.DefaultDisplayName;
                }

                dynamicModuleViewModel.CustomConfigurationViewModel.ParentPropertyName =
                    nameof(dynamicModuleViewModel.CustomConfigurationViewModel);

                var viewModelWithMainLogo = dynamicModuleViewModel.CustomConfigurationViewModel as IModuleMainLogoViewModel;

                if (viewModelWithMainLogo != null)
                {
                    var mainLogoDynamicModuleBlob = dynamicModule.DynamicModuleBlobs
                        .FirstOrDefault(x => x.Type == DynamicModuleBlobEnumModel.MainLogo);

                    if (mainLogoDynamicModuleBlob != null)
                    {
                        viewModelWithMainLogo.MainLogoFileName = m_fileResourceManager.ResolveFullPath(mainLogoDynamicModuleBlob.File);
                    }
                }
            }

            return dynamicModuleViewModel;
        }

        public EditableApiAccessKeyViewModel BuildApiAccessKeyViewModel(ModelStateDictionary modelState, ApiAccessKeyViewModel apiAccessKey = null)
        {
            var apiPermissions = Enum.GetValues(typeof(ApiAccessPermissionEnumViewModel)).Cast<ApiAccessPermissionEnumViewModel>();
            var selectableApiPermissions = m_viewModelFactory.GetSelectableViewmodelList(apiPermissions);

            var vm = m_viewModelFactory.GetEditableApiAccessKeyViewModel(selectableApiPermissions, apiAccessKey);
            vm.EditableApiAccessKeyHashViewModel = BuildApiAccessKeyHashViewModel(modelState, apiAccessKey?.ApiAccessKeyHashViewModel);

            return vm;
        }

        public EditableApiAccessKeyHashViewModel BuildApiAccessKeyHashViewModel(ModelStateDictionary modelState,
            ApiAccessKeyHashViewModel editableApiAccessKeyHash = null)
        {
            var algorithmList = m_hashManager.GetAllAlgorithms();

            return m_viewModelFactory.GetEditableApiAccessKeyHashViewModel(algorithmList, editableApiAccessKeyHash);
        }
    }
}
