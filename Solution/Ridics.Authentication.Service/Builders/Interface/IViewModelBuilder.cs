using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

namespace Ridics.Authentication.Service.Builders.Interface
{
    public interface IViewModelBuilder
    {
        List<SelectableViewModel<RoleViewModel>> BuildUserRolesViewModel(ModelStateDictionary modelState, int id);

        EditableClaimViewModel BuildAddClaimViewModel(ModelStateDictionary modelState, int id);

        EditableUserViewModel<UserWithPasswordViewModel> BuildCreateUserViewModel(
            ModelStateDictionary modelState,
            UserWithPasswordViewModel viewModel = null
        );

        Task<EditableUserViewModel<UserViewModel>> BuildEditableUserViewModelAsync(
            ModelStateDictionary modelState,
            UserViewModel viewModel = null
        );

        List<SelectableViewModel<PermissionViewModel>> BuildPermissionsViewModel(ModelStateDictionary modelState, int id);

        List<SelectableViewModel<ResourcePermissionViewModel>> BuildRoleResourcePermissionsViewModel(ModelStateDictionary modelState, int id);

        List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildRoleResourcePermissionTypeActionsViewModel(ModelStateDictionary modelState, int id);

        List<SelectableViewModel<ResourcePermissionViewModel>> BuildUserResourcePermissionsViewModel(ModelStateDictionary modelState, int id);

        List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildUserResourcePermissionTypeActionsViewModel(ModelStateDictionary modelState, int id);

        IList<SelectableViewModel<RoleViewModel>> BuildPermissionRolesViewModel(ModelStateDictionary modelState, int id);

        IList<SelectableViewModel<RoleViewModel>> BuildResourcePermissionRolesViewModel(ModelStateDictionary modelState, int id);

        IList<SelectableViewModel<RoleViewModel>> BuildResourcePermissionTypeActionRolesViewModel(ModelStateDictionary modelState, int resourcePermissionTypeActionId);

        IList<SelectableViewModel<ResourcePermissionTypeActionViewModel>> BuildResourcePermissionTypeActionsViewModel(ModelStateDictionary modelState, int resourcePermissionTypeId);

        EditableIdentityResourceViewModel BuildEditableIdentityResourceViewModel(
            ModelStateDictionary modelState,
            IdentityResourceViewModel viewModel = null
        );

        ConsentViewModel BuildConsentViewModel(ModelStateDictionary modelState, AuthorizationRequest request, string returnUrl);

        EditableSecretViewModel BuildClientAddSecretViewModel(
            ModelStateDictionary modelState,
            int clientId,
            EditableSecretViewModel secret = null
        );

        EditableClientViewModel BuildEditableClientViewModel(
            ModelStateDictionary modelState,
            ClientViewModel viewModel = null
        );

        UriViewModel BuildCreateUriViewModel(ModelStateDictionary modelState, int clientId, UriViewModel uri = null);

        ClaimTypeViewModel BuildClaimTypeViewModel(ModelStateDictionary modelState, ClaimTypeViewModel claimTypeViewModel = null);

        EditableApiResourceViewModel BuildEditableApiResourceViewModel(
            ModelStateDictionary modelState,
            ApiResourceViewModel viewModel = null
        );

        EditableSecretViewModel BuildAddSecretViewModel(
            ModelStateDictionary modelState,
            int apiResourceId,
            EditableSecretViewModel secret = null
        );

        EditableScopeViewModel BuildAddScopeViewModel(
            ModelStateDictionary modelState,
            int apiResourceId,
            EditableScopeViewModel scope = null
        );

        CreateDynamicModuleViewModel BuildCreateDynamicModuleViewModel(
            ModelStateDictionary modelState,
            CreateDynamicModuleViewModel viewModel = null
        );

        DynamicModuleViewModel BuildEditDynamicModuleViewModel(
            ModelStateDictionary modelState,
            int dynamicModuleId,
            IDynamicModuleViewModel viewModel = null
        );

        EditableApiAccessKeyViewModel BuildApiAccessKeyViewModel(ModelStateDictionary modelState, ApiAccessKeyViewModel editableApiAccessKey = null);

        EditableApiAccessKeyHashViewModel BuildApiAccessKeyHashViewModel(ModelStateDictionary modelState, ApiAccessKeyHashViewModel editableApiAccessKeyHash = null);
    }
}
