using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class RoleController : AuthControllerBase<RoleController>
    {
        private readonly RoleManager m_rolesManager;

        public RoleController(RoleManager rolesManager)
        {
            m_rolesManager = rolesManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var rolesResult = m_rolesManager.GetRoles(start, count, searchByName, true);
            var itemsCountResult = m_rolesManager.GetRolesCount(searchByName);

            if (rolesResult.HasError)
            {
                ModelState.AddModelError(rolesResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var roleViewModels = Mapper.Map<IList<RoleViewModel>>(rolesResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(roleViewModels,
                Translator.Translate("delete-role-confirm-dialog-title"),
                Translator.Translate("delete-role-confirm-dialog-message"),
                itemsCount);

            if (partial)
            {
                return PartialView("_RoleList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var result = m_rolesManager.FindRoleById(id, true);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<RoleViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-role-confirm-dialog-title"),
                Translator.Translate("delete-role-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new RoleViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var roleModel = Mapper.Map<RoleModel>(roleViewModel);

                var result = m_rolesManager.CreateRole(roleModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = new RoleViewModel();

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_rolesManager.FindRoleById(id, true);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<RoleViewModel>(result.Result);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var roleModel = Mapper.Map<RoleModel>(roleViewModel);

                var result = m_rolesManager.UpdateRole(id, roleModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            roleViewModel.Id = id;

            return View(roleViewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_rolesManager.DeleteRoleWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Permissions(int id)
        {
            var viewModel = ViewModelBuilder.BuildPermissionsViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Permissions(int id, List<SelectableViewModel<PermissionViewModel>> selectablePermissions)
        {
            var permissionsIds = GetSelectedItems(selectablePermissions).Select(x => x.Id);

            var result = m_rolesManager.AssignPermissionsToRole(id, permissionsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildPermissionsViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id});
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult ResourcePermissions(int id)
        {
            var viewModel = ViewModelBuilder.BuildRoleResourcePermissionsViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult ResourcePermissions(int id, List<SelectableViewModel<ResourcePermissionViewModel>> selectablePermissions)
        {
            var permissionsIds = GetSelectedItems(selectablePermissions).Select(x => x.Id);

            var result = m_rolesManager.AssignResourcePermissionsToRole(id, permissionsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildRoleResourcePermissionsViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id});
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult ResourcePermissionTypeActions(int id)
        {
            var viewModel = ViewModelBuilder.BuildRoleResourcePermissionTypeActionsViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult ResourcePermissionTypeActions(int id, List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> selectablePermissions)
        {
            var permissionsIds = GetSelectedItems(selectablePermissions).Select(x => x.Id);

            var result = m_rolesManager.AssignResourcePermissionTypeActionsToRole(id, permissionsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildRoleResourcePermissionTypeActionsViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id = id});
        }
    }
}
