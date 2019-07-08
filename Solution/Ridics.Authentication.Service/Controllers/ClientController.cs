using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class ClientController : AuthControllerBase<ClientController>
    {
        private readonly ClientManager m_clientManager;
        private readonly UriManager m_uriManager;
        private readonly SecretManager m_secretManager;


        public ClientController(ClientManager clientManager, UriManager uriManager, SecretManager secretManager)
        {
            m_clientManager = clientManager;
            m_uriManager = uriManager;
            m_secretManager = secretManager;
        }

        [HttpGet]
        public ActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var clientsResult = m_clientManager.GetClients(start, count, searchByName);
            var itemsCountResult = m_clientManager.GetClientsCount(searchByName);

            if (clientsResult.HasError)
            {
                ModelState.AddModelError(clientsResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var clientViewModels = Mapper.Map<IList<ClientViewModel>>(clientsResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(clientViewModels,
                Translator.Translate("delete-client-confirm-dialog-title"),
                Translator.Translate("delete-client-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_ClientList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult View(int id)
        {
            var result = m_clientManager.FindClientById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<ClientViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-client-confirm-dialog-title"),
                Translator.Translate("delete-client-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildEditableClientViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EditableClientViewModel editableClient)
        {
            if (ModelState.IsValid)
            {
                var selectedIdentityResources = GetSelectedItems(editableClient.SelectableIdentityresources);
                var selectedGrantTypes = GetSelectedItems(editableClient.SelectableGrantTypes);
                var selectedScopes = GetSelectedItems(editableClient.SelectableScopes);

                var identityResourcesIds = selectedIdentityResources?.Select(x => x.Id);
                var grantTypesIds = selectedGrantTypes.Select(x => x.Id);
                var scopesIds = selectedScopes.Select(x => x.Id);

                try
                {
                    Client.ValidateGrantTypes(selectedGrantTypes.Select(x => x.Value));

                    var clientModel = Mapper.Map<ClientModel>(editableClient);

                    var result = m_clientManager.CreateClient(clientModel, identityResourcesIds, grantTypesIds, scopesIds);

                    if (!result.HasError)
                    {
                        return RedirectToAction(nameof(View), new { id = result.Result });
                    }

                    ModelState.AddModelError(result.Error.Message);
                }
                catch (InvalidOperationException e)
                {
                    //TODO log warning here
                    ModelState.AddModelError(e.Message); //HACK write custom message
                }
            }

            var viewModel = ViewModelBuilder.BuildEditableClientViewModel(ModelState, editableClient);

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id)
        {
            var result = m_clientManager.FindClientById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var clientViewModel = Mapper.Map<ClientViewModel>(result.Result);

            var viewModel = ViewModelBuilder.BuildEditableClientViewModel(ModelState, clientViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Edit(int id, EditableClientViewModel clientViewModel)
        {
            if (ModelState.IsValid)
            {
                var selectedIdentityResources = GetSelectedItems(clientViewModel.SelectableIdentityresources);
                var selectedGrantTypes = GetSelectedItems(clientViewModel.SelectableGrantTypes);
                var selectedScopes = GetSelectedItems(clientViewModel.SelectableScopes);

                var identityResourcesIds = selectedIdentityResources?.Select(x => x.Id);
                var grantTypesIds = selectedGrantTypes.Select(x => x.Id);
                var scopesIds = selectedScopes.Select(x => x.Id);

                try
                {
                    Client.ValidateGrantTypes(selectedGrantTypes.Select(x => x.Value));

                    var clientModel = Mapper.Map<ClientModel>(clientViewModel);

                    var result = m_clientManager.UpdateClient(id, clientModel, identityResourcesIds, grantTypesIds, scopesIds);

                    if (!result.HasError)
                    {
                        return RedirectToAction(nameof(View), new {id});
                    }

                    ModelState.AddModelError(result.Error.Message);
                }
                catch (InvalidOperationException e)
                {
                    //TODO log warning here
                    ModelState.AddModelError(e.Message); //HACK wrtie custom message
                }
            }

            clientViewModel.Id = id;

            var viewModel = ViewModelBuilder.BuildEditableClientViewModel(ModelState, clientViewModel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        public ActionResult Delete(int id)
        {
            var result = m_clientManager.DeleteClientWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{clientId}/[action]")]
        public IActionResult Uris(int clientId)
        {
            LoadCachedModelState();
            var urisResult = m_uriManager.FindUrisForClient(clientId);

            if (urisResult.HasError)
            {
                ModelState.AddModelError(urisResult.Error.Message);
                return View();
            }

            var uriViewModels = Mapper.Map<IList<UriViewModel>>(urisResult.Result);

            var vm = ViewModelFactory.GetListViewModel(uriViewModels,
                Translator.Translate("delete-uri-confirm-dialog-title"),
                Translator.Translate("delete-uri-confirm-dialog-message"),
                urisResult.Result.Count);

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{clientId}/[action]/")]
        public IActionResult CreateUri(int clientId)
        {
            var viewModel = ViewModelBuilder.BuildCreateUriViewModel(ModelState, clientId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{clientId}/[action]/")]
        public IActionResult CreateUri(int clientId, UriViewModel uri)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = ViewModelBuilder.BuildCreateUriViewModel(ModelState, clientId, uri);

                return View(viewModel);
            }

            var uriModel = Mapper.Map<UriModel>(uri);

            var result = m_uriManager.CreateUriForClient(clientId, uriModel);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildCreateUriViewModel(ModelState, clientId);

                return View(viewModel);
            }

            return RedirectToAction(nameof(Uris), new {clientId});
        }

        [HttpPost]
        [Route("[controller]/{clientId}/[action]/{uriId}")]
        public IActionResult RemoveUri(int clientId, int uriId)
        {
            var result = m_uriManager.DeleteUriForClient(uriId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Uris), new {clientId});
        }

        [HttpGet]
        [Route("[controller]/{clientId}/[action]")]
        public IActionResult Secrets(int clientId)
        {
            LoadCachedModelState();
            var secretsResult = m_secretManager.GetSecretsForClient(clientId);

            if (secretsResult.HasError)
            {
                ModelState.AddModelError(secretsResult.Error.Message);
                return View();
            }

            var secretViewModels = Mapper.Map<IList<SecretViewModel>>(secretsResult.Result);

            var vm = ViewModelFactory.GetClientSecretsViewModel(secretViewModels, clientId,
                Translator.Translate("delete-secret-confirm-dialog-title"),
                Translator.Translate("delete-secret-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{clientId}/[action]")]
        public IActionResult AddSecret(int clientId)
        {
            var viewModel = ViewModelBuilder.BuildClientAddSecretViewModel(ModelState, clientId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{clientId}/[action]")]
        public IActionResult AddSecret(int clientId, EditableSecretViewModel secret)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = ViewModelBuilder.BuildClientAddSecretViewModel(ModelState, clientId, secret);

                return View(viewModel);
            }

            var secretModel = Mapper.Map<SecretModel>(secret);

            var result = m_secretManager.AddSecretToClient(clientId, secretModel);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildClientAddSecretViewModel(ModelState, clientId, secret);

                return View(viewModel);
            }

            return RedirectToAction(nameof(Secrets), new {clientId});
        }

        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{clientId}/[action]/{secretId}")]
        public IActionResult DeleteSecret(int clientId, int secretId)
        {
            var result = m_secretManager.DeleteSecret(secretId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Secrets), new {clientId});
        }
    }
}
