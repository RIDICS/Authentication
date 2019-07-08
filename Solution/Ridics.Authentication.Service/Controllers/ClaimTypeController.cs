using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class ClaimTypeController : AuthControllerBase<ClaimTypeController>
    {
        private readonly ClaimTypeManager m_claimTypeManager;
        private readonly IMapper m_mapper;

        public ClaimTypeController(
            ClaimTypeManager claimTypeManager,
            IMapper mapper
        )
        {
            m_claimTypeManager = claimTypeManager;
            m_mapper = mapper;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var claimTypesResult = m_claimTypeManager.GetClaimTypes(start, count, searchByName);
            var itemsCountResult = m_claimTypeManager.GetClaimTypesCount(searchByName);

            if (claimTypesResult.HasError)
            {
                ModelState.AddModelError(claimTypesResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var claimTypeViewModels = m_mapper.Map<List<ClaimTypeViewModel>>(claimTypesResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(claimTypeViewModels,
                Translator.Translate("delete-claim-type-confirm-dialog-title"),
                Translator.Translate("delete-claim-type-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_ClaimTypeList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var claimType = m_claimTypeManager.FindClaimTypeById(id);

            if (claimType.HasError)
            {
                ModelState.AddModelError(claimType.Error.Message);
            }

            var claimTypeViewModel = Mapper.Map<ClaimTypeViewModel>(claimType.Result);

            var viewModel = ViewModelBuilder.BuildClaimTypeViewModel(ModelState, claimTypeViewModel);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-claim-type-confirm-dialog-title"),
                Translator.Translate("delete-claim-type-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildClaimTypeViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClaimTypeViewModel claimTypeViewModel)
        {
            if (ModelState.IsValid)
            {
                var claimTypeModel = m_mapper.Map<ClaimTypeModel>(claimTypeViewModel);

                var result = m_claimTypeManager.CreateClaimType(claimTypeModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id = result.Result });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildClaimTypeViewModel(ModelState, claimTypeViewModel);

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var claimType = m_claimTypeManager.FindClaimTypeById(id);

            if (claimType.HasError)
            {
                ModelState.AddModelError(claimType.Error.Message);
            }

            var claimTypeViewModel = m_mapper.Map<ClaimTypeViewModel>(claimType.Result);

            var viewModel = ViewModelBuilder.BuildClaimTypeViewModel(ModelState, claimTypeViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(ClaimTypeViewModel claimType)
        {
            if (ModelState.IsValid)
            {
                var claimTypeModel = m_mapper.Map<ClaimTypeModel>(claimType);

                var result = m_claimTypeManager.UpdateClaimType(claimTypeModel);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(View), new { id  = claimType.Id });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var viewModel = ViewModelBuilder.BuildClaimTypeViewModel(ModelState, claimType);

            return View(viewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_claimTypeManager.DeleteClaimTypeWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        public int GetClaimTypesCount()
        {
            return m_claimTypeManager.GetAllClaimTypes().Result.Count;
        }
    }
}
