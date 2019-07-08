using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.DynamicModule;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Helpers.DynamicModule;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.Models.ViewModel.DynamicModule;
using Ridics.Authentication.Service.Utils;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class DynamicModuleController : AuthControllerBase<DynamicModuleController>
    {
        private readonly DynamicModuleManager m_dynamicModuleManager;
        private readonly DynamicModuleConfiguration m_dynamicModuleConfiguration;
        private readonly DynamicModuleProvider m_dynamicModuleProvider;
        private readonly DynamicModuleConfigurationManager m_dynamicConfigurationManager;
        private readonly IMapper m_mapper;

        public DynamicModuleController(
            DynamicModuleManager dynamicModuleManager,
            DynamicModuleConfiguration dynamicModuleConfiguration,
            DynamicModuleProvider dynamicModuleProvider,
            DynamicModuleConfigurationManager dynamicConfigurationManager,
            IMapper mapper
        )
        {
            m_dynamicModuleManager = dynamicModuleManager;
            m_dynamicModuleConfiguration = dynamicModuleConfiguration;
            m_dynamicModuleProvider = dynamicModuleProvider;
            m_dynamicConfigurationManager = dynamicConfigurationManager;
            m_mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(
            int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage,
            bool partial = false
        )
        {
            var itemsCount = m_dynamicModuleManager.GetDynamicModuleCount();

            var userViewModelsRequest = m_dynamicModuleManager.FindAllDynamicModule(start, count);

            var userViewModels = m_mapper.Map<IList<DynamicModuleViewModel>>(userViewModelsRequest.Result);

            var vm = ViewModelFactory.GetListViewModel(
                userViewModels,
                Translator.Translate("delete-dynamic-module-confirm-dialog-title"),
                Translator.Translate("delete-dynamic-module-confirm-dialog-message"),
                itemsCount
            );

            var viewModel = new DynamicModuleListViewModel
            {
                ListViewModel = vm,
                ApplyChangesConfirmDialog = ViewModelFactory.GetConfirmDialogViewmodel(
                    "apply-changes-confirm-dialog",
                    Translator.Translate("dynamic-module-apply-changes-confirm-dialog-title"),
                    Translator.Translate("dynamic-module-apply-changes-confirm-dialog-message")
                ),
                LastConfigurationReload = m_dynamicModuleConfiguration.LastConfigurationReload
            };

            if (partial)
            {
                return PartialView("_DynamicModuleList", viewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildCreateDynamicModuleViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create([FromForm] CreateDynamicModuleViewModel viewModel)
        {
            if (
                ModelState.IsValid
                && (
                    !string.IsNullOrEmpty(viewModel.NameOption)
                    || !string.IsNullOrEmpty(viewModel.Name)
                )
            )
            {
                var dynamicModuleModel = m_mapper.Map<DynamicModuleModel>(viewModel);

                var moduleInfo = m_dynamicModuleProvider.GetLibraryModuleInfos()
                    .FirstOrDefault(x => x.ModuleGuid == dynamicModuleModel.ModuleGuid);

                //TODO introduce validating service: unique Name (related to Guid)

                var result = m_dynamicModuleManager.CreateDynamicModule(dynamicModuleModel, moduleInfo);

                if (!result.HasError)
                {
                    return RedirectToAction(nameof(Edit), new
                    {
                        id = result.Result
                    });
                }

                ModelState.AddModelError(result.Error.Message);
            }

            var defaultViewModel = ViewModelBuilder.BuildCreateDynamicModuleViewModel(ModelState, viewModel);

            return View("Create", defaultViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var viewModel = ViewModelBuilder.BuildEditDynamicModuleViewModel(ModelState, id);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var result = m_dynamicModuleManager.DeleteDynamicModuleWithId(id);

            if (result.HasError)
            {
                //TODO With PRG pattern, error after redirecting to index is lost, its possible to serialize it: https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/
                ModelState.AddModelError(result.Error.Message);
                return View(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.RestartAuthServicePolicy)]
        public IActionResult ApplyChanges()
        {
            m_dynamicConfigurationManager.DumpConfigurationAndRestartIfChanged();

            return RedirectToAction(nameof(Index));
        }
    }

    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    [DynamicModuleControllerName]
    public class GenericDynamicModuleController<T> : AuthControllerBase<DynamicModuleController>
        where T : class, IModuleConfigurationViewModel
    {
        private readonly DynamicModuleManager m_dynamicModuleManager;
        private readonly DynamicModuleProvider m_dynamicModuleProvider;

        public GenericDynamicModuleController(
            DynamicModuleManager dynamicModuleManager,
            DynamicModuleProvider dynamicModuleProvider
        )
        {
            m_dynamicModuleManager = dynamicModuleManager;
            m_dynamicModuleProvider = dynamicModuleProvider;
        }

        [HttpGet("[controller]/[action]/{id}")]
        public IActionResult Edit([FromRoute] int id)
        {
            return RedirectToAction("Edit", "DynamicModule", new
            {
                id
            });
        }

        [HttpPost("[controller]/[action]/{id}")]
        public IActionResult Edit(DynamicModuleViewModel<T> configuration)
        {
            var dynamicModuleModelRequest = m_dynamicModuleManager.GetById(configuration.Id);

            if (dynamicModuleModelRequest.HasError)
            {
                return NotFound(configuration.Id);
            }

            var dynamicModuleModel = dynamicModuleModelRequest.Result;

            var moduleInfo = m_dynamicModuleProvider.GetContextByNameOrGuid(
                dynamicModuleModel.Name,
                dynamicModuleModel.ModuleGuid
            );

            if (moduleInfo == null)
            {
                return NotFound(configuration.Id);
            }

            var configurationDto = moduleInfo.ModuleConfiguration ?? moduleInfo.EmptyModuleConfiguration;

            configuration.CustomConfigurationViewModel.Name = configuration.Name;

            moduleInfo.ModuleConfigurationManager.HydrateModuleConfiguration(
                configurationDto,
                configuration.CustomConfigurationViewModel
            );

            m_dynamicModuleManager.UpdateConfiguration(
                configuration.Id,
                configurationDto,
                moduleInfo.LibModuleInfo.Version
            );

            var viewModelWithMainLogo = configuration.CustomConfigurationViewModel as IModuleMainLogoViewModel;

            if (viewModelWithMainLogo?.MainLogo != null)
            {
                if (ContentType.ImageContentTypes.Contains(viewModelWithMainLogo.MainLogo.ContentType))
                {
                    var dynamicModuleBlobRequest = m_dynamicModuleManager.GetDynamicModuleBlob(
                        configuration.Id,
                        DynamicModuleBlobEnum.MainLogo
                    );

                    var mainLogoStream = viewModelWithMainLogo.MainLogo.OpenReadStream();
                    var mainLogoFileExtension = Path.GetExtension(viewModelWithMainLogo.MainLogo.FileName).Substring(1).ToLower();

                    var dynamicModuleBlob = dynamicModuleBlobRequest.Result;

                    if (dynamicModuleBlobRequest.Succeeded && dynamicModuleBlob != null)
                    {
                        m_dynamicModuleManager.UpdateDynamicModuleBlob(
                            dynamicModuleBlob.Id,
                            mainLogoStream,
                            mainLogoFileExtension
                        );
                    }
                    else
                    {
                        m_dynamicModuleManager.CreateDynamicModuleBlob(
                            configuration.Id,
                            DynamicModuleBlobEnum.MainLogo,
                            mainLogoStream,
                            mainLogoFileExtension
                        );
                    }
                }
            }

            return RedirectToAction("Edit", "DynamicModule", new
            {
                id = configuration.Id
            });
        }
    }
}
