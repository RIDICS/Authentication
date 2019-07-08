using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel.Keys;

namespace Ridics.Authentication.Service.Controllers
{
    public class ApiAccessKeyController : AuthControllerBase<ApiAccessKeyController>
    {
        private readonly ApiAccessKeyManager m_apiAccessKeyManager;
        private readonly HashManager m_hashManager;

        public ApiAccessKeyController(ApiAccessKeyManager apiAccessKeyManager, HashManager hashManager)
        {
            m_apiAccessKeyManager = apiAccessKeyManager;
            m_hashManager = hashManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex, int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();

            var apiAccessKeysResult = m_apiAccessKeyManager.GetApiAccessKeys(start, count, searchByName);
            var itemsCountResult = m_apiAccessKeyManager.GetApiAccessKeysCount(searchByName);

            if (apiAccessKeysResult.HasError)
            {
                ModelState.AddModelError(apiAccessKeysResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var apiAccessKeyViewModels = Mapper.Map<List<ApiAccessKeyViewModel>>(apiAccessKeysResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(apiAccessKeyViewModels,
                Translator.Translate("delete-api-access-key-confirm-dialog-title"),
                Translator.Translate("delete-api-access-key-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_ApiAccessKeyList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var apiAccessKey = m_apiAccessKeyManager.FindApiAccessKeyById(id);

            if (apiAccessKey.HasError)
            {
                ModelState.AddModelError(apiAccessKey.Error.Message);
            }

            var apiAccessKeyViewModel = Mapper.Map<ApiAccessKeyViewModel>(apiAccessKey.Result);

            var vm = ViewModelFactory.GetViewModel(apiAccessKeyViewModel,
                Translator.Translate("delete-api-access-key-confirm-dialog-title"),
                Translator.Translate("delete-api-access-key-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var vm = ViewModelBuilder.BuildApiAccessKeyViewModel(ModelState);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditableApiAccessKeyViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedPermissions = GetSelectedItems(editableViewModel.SelectableApiPermissions).Select(x => (ApiAccessPermissionEnumModel)x);
                var apiAccessKeyModel = Mapper.Map<ApiAccessKeyModel>(editableViewModel);

                apiAccessKeyModel.ApiKeyHash = m_hashManager.GenerateHash(editableViewModel.EditableApiAccessKeyHashViewModel.Value, editableViewModel.EditableApiAccessKeyHashViewModel.Algorithm);

                var result = m_apiAccessKeyManager.CreateApiAccessKey(apiAccessKeyModel, selectedPermissions);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var vm = ViewModelBuilder.BuildApiAccessKeyViewModel(ModelState, editableViewModel);

            return View(vm);
        }


        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var apiAccessKeyModel = m_apiAccessKeyManager.FindApiAccessKeyById(id);

            if (apiAccessKeyModel.HasError)
            {
                ModelState.AddModelError(apiAccessKeyModel.Error.Message);
                return View();
            }

            var apiAccessKeyViewModel = Mapper.Map<ApiAccessKeyViewModel>(apiAccessKeyModel.Result);
            var viewModel = ViewModelBuilder.BuildApiAccessKeyViewModel(ModelState, apiAccessKeyViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditableApiAccessKeyViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedPermissions = GetSelectedItems(editableViewModel.SelectableApiPermissions).Select(x => (ApiAccessPermissionEnumModel)x);
                var apiAccessKeyModel = Mapper.Map<ApiAccessKeyModel>(editableViewModel);
                
                var result = m_apiAccessKeyManager.UpdateApiAccessKey(id, apiAccessKeyModel, selectedPermissions);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildApiAccessKeyViewModel(ModelState, editableViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_apiAccessKeyManager.DeleteApiAccessKeyWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public IActionResult EditKeyHash(int id)
        {
            var apiAccessKeyModel = m_apiAccessKeyManager.FindApiAccessKeyById(id);

            if (apiAccessKeyModel.HasError)
            {
                ModelState.AddModelError(apiAccessKeyModel.Error.Message);
                return View();
            }

            var apiAccessKeyViewModel = Mapper.Map<ApiAccessKeyHashViewModel>(apiAccessKeyModel.Result);
            var viewModel = ViewModelBuilder.BuildApiAccessKeyHashViewModel(ModelState, apiAccessKeyViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult EditKeyHash(int id, EditableApiAccessKeyHashViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var hash = m_hashManager.GenerateHash(editableViewModel.Value, editableViewModel.Algorithm);
                
                var result = m_apiAccessKeyManager.UpdateApiAccessKeyHash(id, hash, editableViewModel.Algorithm);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildApiAccessKeyHashViewModel(ModelState, editableViewModel);

            return View(viewModel);
        }
    }
}