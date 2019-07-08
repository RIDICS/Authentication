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
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class ApiResourceController : AuthControllerBase<ApiResourceController>
    {
        private readonly ApiResourceManager m_apiResourceManager;
        private readonly SecretManager m_secretManager;
        private readonly ScopeManager m_scopeManager;

        public ApiResourceController(ApiResourceManager apiResourceManager, SecretManager secretManager, ScopeManager scopeManager)
        {
            m_apiResourceManager = apiResourceManager;
            m_secretManager = secretManager;
            m_scopeManager = scopeManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex, int count = PaginationConstants.ItemsOnPage,
            string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var apiResourcesResult = m_apiResourceManager.GetApiResources(start, count, searchByName);
            var itemsCountResult = m_apiResourceManager.GetApiResourcesCount(searchByName);

            if (apiResourcesResult.HasError)
            {
                ModelState.AddModelError(apiResourcesResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var apiResourceViewModels = Mapper.Map<List<ApiResourceViewModel>>(apiResourcesResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(apiResourceViewModels,
                Translator.Translate("delete-api-resource-confirm-dialog-title"),
                Translator.Translate("delete-api-resource-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_ApiResourceList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var identityResourceResult = m_apiResourceManager.FindApiResourceById(id);

            if (identityResourceResult.HasError)
            {
                ModelState.AddModelError(identityResourceResult.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ApiResourceViewModel>(identityResourceResult.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-api-resource-confirm-dialog-title"),
                Translator.Translate("delete-api-resource-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildEditableApiResourceViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditableApiResourceViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimsIds = GetSelectedItems(editableViewModel.SelectableClaimTypes).Select(x => x.Id);
                var apiResourceModel = Mapper.Map<ApiResourceModel>(editableViewModel.ApiResourceViewModel);

                var result = m_apiResourceManager.CreateApiResource(apiResourceModel, claimsIds);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id = result.Result});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildEditableApiResourceViewModel(ModelState, editableViewModel.ApiResourceViewModel);

            return View(viewModel);
        }


        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var apiResourceResult = m_apiResourceManager.FindApiResourceById(id);

            if (apiResourceResult.HasError)
            {
                ModelState.AddModelError(apiResourceResult.Error.Message);
                return View();
            }

            var apiResourceViewModel = Mapper.Map<ApiResourceViewModel>(apiResourceResult.Result);
            var viewModel = ViewModelBuilder.BuildEditableApiResourceViewModel(ModelState, apiResourceViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditableApiResourceViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimsIds = GetSelectedItems(editableViewModel.SelectableClaimTypes).Select(x => x.Id);
                var apiResourceModel = Mapper.Map<ApiResourceModel>(editableViewModel.ApiResourceViewModel);

                var result = m_apiResourceManager.UpdateApiResource(id, apiResourceModel, claimsIds);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildEditableApiResourceViewModel(ModelState, editableViewModel.ApiResourceViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_apiResourceManager.DeleteApiResourceWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult Secrets(int apiResourceId)
        {
            LoadCachedModelState();
            var secretsResult = m_secretManager.GetSecretsForApiResources(apiResourceId);

            if (secretsResult.HasError)
            {
                ModelState.AddModelError(secretsResult.Error.Message);
                return View();
            }

            var secretViewModels = Mapper.Map<IList<SecretViewModel>>(secretsResult.Result);

            var vm = ViewModelFactory.GetApiResourceSecretsViewModel(secretViewModels, apiResourceId,
                Translator.Translate("delete-secret-confirm-dialog-title"),
                Translator.Translate("delete-secret-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult AddSecret(int apiResourceId)
        {
            var viewModel = ViewModelBuilder.BuildAddSecretViewModel(ModelState, apiResourceId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult AddSecret(int apiResourceId, EditableSecretViewModel secret)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = ViewModelBuilder.BuildAddSecretViewModel(ModelState, apiResourceId, secret);

                return View(viewModel);
            }

            var secretModel = Mapper.Map<SecretModel>(secret);

            var result = m_secretManager.AddSecretToApiResource(apiResourceId, secretModel);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildAddSecretViewModel(ModelState, apiResourceId, secret);

                return View(viewModel);
            }

            return RedirectToAction(nameof(Secrets), new {apiResourceId});
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{apiResourceId}/[action]/{secretId}")]
        public IActionResult DeleteSecret(int apiResourceId, int secretId)
        {
            var result = m_secretManager.DeleteSecret(secretId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Secrets), new {apiResourceId});
        }

        [HttpGet]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult Scopes(int apiResourceId)
        {
            LoadCachedModelState();
            var scopesResult = m_scopeManager.GetScopesForApiResources(apiResourceId);

            if (scopesResult.HasError)
            {
                ModelState.AddModelError(scopesResult.Error.Message);
                return View();
            }

            var scopeViewModels = Mapper.Map<IList<ScopeViewModel>>(scopesResult.Result);

            var vm = ViewModelFactory.GetApiResourceScopesViewModel(scopeViewModels, apiResourceId,
                Translator.Translate("delete-scope-confirm-dialog-title"),
                Translator.Translate("delete-scope-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult AddScope(int apiResourceId)
        {
            var viewModel = ViewModelBuilder.BuildAddScopeViewModel(ModelState, apiResourceId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{apiResourceId}/[action]")]
        public IActionResult AddScope(int apiResourceId, EditableScopeViewModel scope)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = ViewModelBuilder.BuildAddScopeViewModel(ModelState, apiResourceId);

                return View(viewModel);
            }

            var claimsIds = GetSelectedItems(scope.SelectableClaimTypes).Select(x => x.Id);
            var scopeModel = Mapper.Map<ScopeModel>(scope);

            var result = m_scopeManager.AddScopeToApiResource(apiResourceId, scopeModel, claimsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildAddScopeViewModel(ModelState, apiResourceId);

                return View(viewModel);
            }

            return RedirectToAction(nameof(Scopes), new {apiResourceId});
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{apiResourceId}/[action]/{scopeId}")]
        public IActionResult DeleteScope(int apiResourceId, int scopeId)
        {
            var result = m_scopeManager.DeleteScope(scopeId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Scopes), new {apiResourceId});
        }
    }
}
