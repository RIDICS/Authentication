using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Models.ViewModel.Account;

namespace Ridics.Authentication.Service.Controllers
{
    public class ConsentController : AuthControllerBase<ConsentController>
    {
        private readonly IEventService m_events;
        private readonly IIdentityServerInteractionService m_interaction;

        public ConsentController(IIdentityServerInteractionService interaction, IEventService events)
        {
            m_interaction = interaction;
            m_events = events;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var request = await m_interaction.GetAuthorizationContextAsync(returnUrl);

            if (request == null)
            {
                ModelState.AddModelError(Translator.Translate("error-occured"));
                return View();
            }

            var vm = ViewModelBuilder.BuildConsentViewModel(ModelState, request, returnUrl);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentViewModel model)
        {
            var request = await m_interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (request == null)
            {
                ModelState.AddModelError(Translator.Translate("error-occured"));
                return View();
            }

            var identityResourcesConfirmed = GetSelectedItems(model.IdentityResources).Select(x => x.Name);
            var scopesConfirmed = GetSelectedItems(model.Scopes).Select(x => x.Name);

            var scopes = new List<string>();

            scopes.AddRange(identityResourcesConfirmed);
            scopes.AddRange(scopesConfirmed);

            var grantedConsent = new ConsentResponse
            {
                RememberConsent = model.RememberConsent,
                ScopesConsented = scopes,
            };

            await m_events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.ClientId, request.ScopesRequested,
                grantedConsent.ScopesConsented, grantedConsent.RememberConsent));

            await m_interaction.GrantConsentAsync(request, grantedConsent);

            return Redirect(model.ReturnUrl);
        }
    }
}