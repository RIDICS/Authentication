using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class ResourcePermissionController : AuthControllerBase<ResourcePermissionController>
    {
        private readonly ResourcePermissionManager m_resourcePermissionManager;
        private readonly ResourcePermissionTypeActionManager m_resourcePermissionTypeActionManager;

        public ResourcePermissionController(ResourcePermissionManager resourcePermissionsManager,
            ResourcePermissionTypeActionManager resourcePermissionsTypeActionManager)
        {
            m_resourcePermissionManager = resourcePermissionsManager;
            m_resourcePermissionTypeActionManager = resourcePermissionsTypeActionManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, bool partial = false)
        {
            LoadCachedModelState();
            var permissionsResult = m_resourcePermissionManager.GetPermissions(start, count);
            var permissionsCountResult = m_resourcePermissionManager.GetPermissionsCount();

            if (permissionsResult.HasError)
            {
                ModelState.AddModelError(permissionsResult.Error.Message);
                return View();
            }

            if (permissionsCountResult.HasError)
            {
                ModelState.AddModelError(permissionsCountResult.Error.Message);
                return View();
            }

            var permissionViewModels = Mapper.Map<IList<ResourcePermissionViewModel>>(permissionsResult.Result);

            var vm = ViewModelFactory.GetListViewModel(permissionViewModels,
                Translator.Translate("delete-permission-confirm-dialog-title"),
                Translator.Translate("delete-permission-confirm-dialog-message"),
                permissionsCountResult.Result);

            if (partial)
            {
                return PartialView("_PermissionList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var result = m_resourcePermissionManager.FindPermissionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-permission-confirm-dialog-title"),
                Translator.Translate("delete-permission-confirm-dialog-message"));

            return View(vm);
        }

        public ActionResult Create()
        {
            return View(CreateEditableViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditResourcePermissionViewModel permissionViewModel)
        {
            var permissionType =
                m_resourcePermissionTypeActionManager.FindPermissionTypeActionById(permissionViewModel.SelectedResourcePermissionTypeActionId);
            permissionViewModel.ResourcePermissionTypeAction = Mapper.Map<ResourcePermissionTypeActionViewModel>(permissionType.Result);

            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionModel>(permissionViewModel);

                var result = m_resourcePermissionManager.CreatePermission(permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            return View(CreateEditableViewModel());
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_resourcePermissionManager.FindPermissionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionViewModel>(result.Result);

            return View(CreateEditableViewModel(viewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditResourcePermissionViewModel permissionViewModel)
        {
            var permissionType =
                m_resourcePermissionTypeActionManager.FindPermissionTypeActionById(permissionViewModel.SelectedResourcePermissionTypeActionId);
            permissionViewModel.ResourcePermissionTypeAction = Mapper.Map<ResourcePermissionTypeActionViewModel>(permissionType.Result);

            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionModel>(permissionViewModel);

                var result = m_resourcePermissionManager.UpdatePermission(id, permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            permissionViewModel.Id = id;

            return View(permissionViewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_resourcePermissionManager.DeletePermissionWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Roles(int id)
        {
            var viewModel = ViewModelBuilder.BuildResourcePermissionRolesViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Roles(int id, List<SelectableViewModel<RoleViewModel>> selectableRoles)
        {
            var selectedRoles = GetSelectedItems(selectableRoles).Select(x => x.Id);

            var result = m_resourcePermissionManager.AssignRolesToPermission(id, selectedRoles);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildResourcePermissionRolesViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id});
        }

        private EditResourcePermissionViewModel CreateEditableViewModel(ResourcePermissionViewModel resourcePermissionViewModel = null)
        {
            var viewModel = resourcePermissionViewModel == null
                ? new EditResourcePermissionViewModel()
                : Mapper.Map<EditResourcePermissionViewModel>(resourcePermissionViewModel);

            var permissionTypeActions = m_resourcePermissionTypeActionManager.GetAllPermissionTypeActions().Result;
            viewModel.ResourcePermissionTypeActionList = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(permissionTypeActions);

            return viewModel;
        }
    }
}