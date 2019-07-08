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
    public class PermissionController : AuthControllerBase<PermissionController>
    {
        private readonly PermissionManager m_permissionManager;

        public PermissionController(PermissionManager permissionsManager)
        {
            m_permissionManager = permissionsManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var permissionsResult = m_permissionManager.GetPermissions(start, count, searchByName);
            var permissionsCountResult = m_permissionManager.GetPermissionsCount(searchByName);

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

            var permissionViewModels = Mapper.Map<IList<PermissionViewModel>>(permissionsResult.Result);

            var vm = ViewModelFactory.GetListViewModel(permissionViewModels,
                Translator.Translate("delete-permission-confirm-dialog-title"),
                Translator.Translate("delete-permission-confirm-dialog-message"),
                permissionsCountResult.Result, count);

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
            var result = m_permissionManager.FindPermissionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<PermissionViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-permission-confirm-dialog-title"),
                Translator.Translate("delete-permission-confirm-dialog-message"));

            return View(vm);
        }

        public ActionResult Create()
        {
            var viewModel = new PermissionViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PermissionViewModel permissionViewModel)
        {
            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<PermissionModel>(permissionViewModel);

                var result = m_permissionManager.CreatePermission(permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id = result.Result});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = new PermissionViewModel();

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_permissionManager.FindPermissionById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<PermissionViewModel>(result.Result);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, PermissionViewModel permissionViewModel)
        {
            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<PermissionModel>(permissionViewModel);

                var result = m_permissionManager.UpdatePermission(id, permissionModel);

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
            var result = m_permissionManager.DeletePermissionWithId(id);

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
            var viewModel = ViewModelBuilder.BuildPermissionRolesViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Roles(int id, List<SelectableViewModel<RoleViewModel>> selectableRoles)
        {
            var selectedRoles = GetSelectedItems(selectableRoles).Select(x => x.Id);

            var result = m_permissionManager.AssignRolesToPermissions(id, selectedRoles);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildPermissionRolesViewModel(ModelState, id);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id});
        }
    }
}
