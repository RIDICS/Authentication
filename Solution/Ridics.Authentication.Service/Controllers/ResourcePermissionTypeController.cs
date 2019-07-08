using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class ResourcePermissionTypeController : AuthControllerBase<ResourcePermissionTypeController>
    {
        private readonly ResourcePermissionTypeManager m_resourcePermissionManager;
        private readonly ResourcePermissionTypeActionManager m_resourcePermissionActionManager;

        public ResourcePermissionTypeController(ResourcePermissionTypeManager resourcePermissionsManager,
            ResourcePermissionTypeActionManager resourcePermissionActionManager)
        {
            m_resourcePermissionManager = resourcePermissionsManager;
            m_resourcePermissionActionManager = resourcePermissionActionManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var permissionsResult = m_resourcePermissionManager.GetPermissionTypes(start, count, searchByName);
            var permissionsCountResult = m_resourcePermissionManager.GetPermissionTypesCount(searchByName);

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

            var permissionViewModels = Mapper.Map<IList<ResourcePermissionTypeViewModel>>(permissionsResult.Result);

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
            var result = m_resourcePermissionManager.FindPermissionTypeById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionTypeViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-permission-confirm-dialog-title"),
                Translator.Translate("delete-permission-confirm-dialog-message"));

            return View(vm);
        }

        public ActionResult Create()
        {
            var viewModel = new ResourcePermissionTypeViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourcePermissionTypeViewModel permissionViewModel)
        {
            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionTypeModel>(permissionViewModel);

                var result = m_resourcePermissionManager.CreatePermissionType(permissionModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id = result.Result});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = new ResourcePermissionTypeViewModel();

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_resourcePermissionManager.FindPermissionTypeById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ResourcePermissionTypeViewModel>(result.Result);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, ResourcePermissionTypeViewModel permissionViewModel)
        {
            if (ModelState.IsValid)
            {
                var permissionModel = Mapper.Map<ResourcePermissionTypeModel>(permissionViewModel);

                var result = m_resourcePermissionManager.UpdatePermissionType(id, permissionModel);

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
            var result = m_resourcePermissionManager.DeletePermissionTypeWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Actions(int id)
        {
            LoadCachedModelState();
            var actionsResult = m_resourcePermissionActionManager.GetActionsForResourcePermissionTypeById(id);

            if (actionsResult.HasError)
            {
                ModelState.AddModelError(actionsResult.Error.Message);
                return View();
            }

            var actionViewModels = Mapper.Map<IList<ResourcePermissionTypeActionViewModel>>(actionsResult.Result);

            var vm = ViewModelFactory.GetPermissionTypeActionsViewModel(actionViewModels, id,
                Translator.Translate("delete-permission-type-confirm-dialog-title"),
                Translator.Translate("delete-permission-type-confirm-dialog-message"));

            return View(vm);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]/{actionId}")]
        public IActionResult DeleteAction(int id, int actionId)
        {
            var result = m_resourcePermissionActionManager.DeletePermissionTypeActionWithId(actionId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Actions), new {id});
        }
    }
}
