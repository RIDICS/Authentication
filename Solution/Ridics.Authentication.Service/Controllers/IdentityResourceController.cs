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
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class IdentityResourceController : AuthControllerBase<IdentityResourceController>
    {
        private readonly IdentityResourceManager m_identityResourceManager;

        public IdentityResourceController(IdentityResourceManager identityResourceManager)
        {
            m_identityResourceManager = identityResourceManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var identityResourcesResult = m_identityResourceManager.GetIdentityResources(start, count, searchByName);
            var itemsCountResult = m_identityResourceManager.GetIdentityResourcesCount(searchByName);

            if (identityResourcesResult.HasError)
            {
                ModelState.AddModelError(identityResourcesResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var identityResourceViewModels =
                Mapper.Map<IList<IdentityResourceViewModel>>(identityResourcesResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(identityResourceViewModels,
                Translator.Translate("delete-identity-resource-confirm-dialog-title"),
                Translator.Translate("delete-identity-resource-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_IdentityResourceList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var identityResourceResult = m_identityResourceManager.FindIdentityResourceById(id);

            if (identityResourceResult.HasError)
            {
                ModelState.AddModelError(identityResourceResult.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<IdentityResourceViewModel>(identityResourceResult.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-identity-resource-confirm-dialog-title"),
                Translator.Translate("delete-identity-resource-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildEditableIdentityResourceViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditableIdentityResourceViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimsIds = GetSelectedItems(editableViewModel.SelectableClaimTypes).Select(x => x.Id);
                var identityResourceModel =
                    Mapper.Map<IdentityResourceModel>(editableViewModel.IdentityResourceViewModel);

                var result = m_identityResourceManager.CreateIdentityResource(identityResourceModel, claimsIds);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel =
                ViewModelBuilder.BuildEditableIdentityResourceViewModel(ModelState,
                    editableViewModel.IdentityResourceViewModel);

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var identityResourceResult = m_identityResourceManager.FindIdentityResourceById(id);

            if (identityResourceResult.HasError)
            {
                ModelState.AddModelError(identityResourceResult.Error.Message);
                return View();
            }

            var identityResourceViewModel = Mapper.Map<IdentityResourceViewModel>(identityResourceResult.Result);

            var viewModel =
                ViewModelBuilder.BuildEditableIdentityResourceViewModel(ModelState, identityResourceViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditableIdentityResourceViewModel editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimsIds = GetSelectedItems(editableViewModel.SelectableClaimTypes).Select(x => x.Id);
                var identityResourceModel =
                    Mapper.Map<IdentityResourceModel>(editableViewModel.IdentityResourceViewModel);

                var result = m_identityResourceManager.UpdateIdentityResource(id, identityResourceModel, claimsIds);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new {id});
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel =
                ViewModelBuilder.BuildEditableIdentityResourceViewModel(ModelState,
                    editableViewModel.IdentityResourceViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_identityResourceManager.DeleteIdentityResourceWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
