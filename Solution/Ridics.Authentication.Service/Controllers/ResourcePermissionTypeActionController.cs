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
    public class ResourcePermissionTypeActionController : AuthControllerBase<ResourcePermissionTypeActionController>
    {
        private readonly ResourcePermissionTypeActionManager m_resourcePermissionManager;
        private readonly ResourcePermissionTypeManager m_resourcePermissionTypeManager;

        public ResourcePermissionTypeActionController(ResourcePermissionTypeActionManager resourcePermissionsManager,
            ResourcePermissionTypeManager resourcePermissionTypesManager)
        {
            m_resourcePermissionManager = resourcePermissionsManager;
            m_resourcePermissionTypeManager = resourcePermissionTypesManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var permissionsResult = m_resourcePermissionManager.GetPermissionTypeActions(start, count, searchByName);
            var permissionsCountResult = m_resourcePermissionManager.GetPermissionTypeActionsCount(searchByName);

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

            ViewData["search"] = searchByName;

            var permissionViewModels = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(permissionsResult.Result);

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
            var result = m_resourcePermissionManager.FindPermissionTypeActionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionTypeActionViewModel>(result.Result);

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
        public ActionResult Create(EditResourcePermissionTypeActionViewModel permissionViewModel)
        {
            var permissionType =
                m_resourcePermissionTypeManager.FindPermissionTypeById(permissionViewModel.SelectedResourcePermissionTypeId);
            permissionViewModel.ResourcePermissionType = Mapper.Map<ResourcePermissionTypeViewModel>(permissionType.Result);

            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionTypeActionModel>(permissionViewModel);

                var result = m_resourcePermissionManager.CreatePermissionTypeAction(permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            return View(CreateEditableViewModel());
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_resourcePermissionManager.FindPermissionTypeActionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionTypeActionViewModel>(result.Result);

            return View(CreateEditableViewModel(viewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditResourcePermissionTypeActionViewModel permissionViewModel)
        {
            var permissionType =
                m_resourcePermissionTypeManager.FindPermissionTypeById(permissionViewModel.SelectedResourcePermissionTypeId);
            permissionViewModel.ResourcePermissionType = Mapper.Map<ResourcePermissionTypeViewModel>(permissionType.Result);

            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionTypeActionModel>(permissionViewModel);

                var result = m_resourcePermissionManager.UpdatePermissionType(id, permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id });
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
            var result = m_resourcePermissionManager.DeletePermissionTypeActionWithId(id);

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
            var viewModel = ViewModelBuilder.BuildResourcePermissionTypeActionRolesViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Roles(int id, List<SelectableViewModel<RoleViewModel>> selectableRoles)
        {
            var selectedRoles = GetSelectedItems(selectableRoles).Select(x => x.Id);

            var result = m_resourcePermissionManager.AssignRolesToPermissionTypeAction(id, selectedRoles);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildResourcePermissionTypeActionRolesViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id});
        }

        private EditResourcePermissionTypeActionViewModel CreateEditableViewModel(ResourcePermissionTypeActionViewModel resourcePermissionViewModel = null)
        {
            var viewModel = resourcePermissionViewModel == null
                ? new EditResourcePermissionTypeActionViewModel()
                : Mapper.Map<EditResourcePermissionTypeActionViewModel>(resourcePermissionViewModel);

            var permissionTypes = m_resourcePermissionTypeManager.GetAllPermissionTypes().Result;
            viewModel.ResourcePermissionTypeList = Mapper.Map<IList<ResourcePermissionTypeViewModel>>(permissionTypes);

            return viewModel;
        }
    }
}