using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Modules.Shared.Model;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Configuration.Enum;
using Ridics.Authentication.Service.Exceptions;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Managers;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.Models.Account;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Core.Structures.Shared;
using Scalesoft.Localization.AspNetCore;

namespace Ridics.Authentication.Service.Controllers
{
    //TODO breakdown and refactor this controller
    [Authorize]
    [LocalizeByParameterFilter]
    public class AccountController : AuthControllerBase<AccountController>
    {
        private const string SsoJavaScriptPath = @"wwwroot\js\ridics.sso.js";

        private readonly IEventService m_events;
        private readonly IdentityUserManager m_identityUserManager;
        private readonly IIdentityServerInteractionService m_interaction;
        private readonly MessageSenderManager m_messageSenderManager;
        private readonly ReturnUrlConfiguration m_returnUrlConfiguration;
        private readonly IdentitySignInManager m_signInManager;
        private readonly UserManager m_userManager;
        private readonly ILocalizationService m_localization;
        private readonly NonceManager m_nonceManager;
        private readonly DynamicModuleProvider m_dynamicModuleProvider;
        private readonly FeatureFlagsManager m_featureFlagsManager;

        public AccountController(
            UserManager userManager,
            IEventService events,
            IIdentityServerInteractionService interaction,
            IdentitySignInManager signInManager,
            IdentityUserManager identityUserManager,
            MessageSenderManager messageSenderManager,
            ReturnUrlConfiguration returnUrlConfiguration,
            ILocalizationService localization,
            NonceManager nonceManager,
            DynamicModuleProvider dynamicModuleProvider,
            FeatureFlagsManager featureFlagsManager
        )
        {
            m_userManager = userManager;
            m_events = events;
            m_interaction = interaction;
            m_signInManager = signInManager;
            m_identityUserManager = identityUserManager;
            m_messageSenderManager = messageSenderManager;
            m_returnUrlConfiguration = returnUrlConfiguration;
            m_localization = localization;
            m_nonceManager = nonceManager;
            m_dynamicModuleProvider = dynamicModuleProvider;
            m_featureFlagsManager = featureFlagsManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? m_returnUrlConfiguration.DefaultRedirectUrl);
            }

            var context = await m_interaction.GetAuthorizationContextAsync(returnUrl);

            if (context != null || Url.IsLocalUrl(returnUrl) || string.IsNullOrEmpty(returnUrl))
            {
                LoadCachedModelState();
                var viewModel = ViewModelFactory.GetLoginViewModel(returnUrl);

                return View(viewModel);
            }

            Logger.LogWarning($"Invalid return url {returnUrl}");

            return BadRequest("Invalid return url");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Required] LoginViewModel viewModel, string buttonClicked)
        {
            var result = await PerformLogInTryout(viewModel);

            return result.ActionResult;
        }

        private async Task<LogInTryoutResult> PerformLogInTryout(LoginViewModel viewModel)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                return new LogInTryoutResult(Redirect(viewModel.ReturnUrl ?? m_returnUrlConfiguration.DefaultRedirectUrl));
            }

            viewModel.ExternalProviders = ViewModelFactory.FindExternalProviderViewModels();

            if (!ModelState.IsValid)
            {
                return new LogInTryoutResult(View(viewModel));
            }

            var username = viewModel.Input.Username;

            var user = m_userManager.GetUserByConfirmedEmail(username); //check if user tried to log in with email

            if (user.Succeeded)
            {
                username = user.Result.Username;
            }

            var loginResult = await m_signInManager.PasswordSignInAsync(
                username, viewModel.Input.Password,
                viewModel.Input.RememberLogin,
                lockoutOnFailure: false
            );

            if (loginResult.RequiresTwoFactor)
            {
                return new LogInTryoutResult(
                    RedirectToAction(
                        nameof(LoginWith2fa),
                        new {viewModel.Input.RememberLogin, viewModel.ReturnUrl}
                    ),
                    false,
                    await m_identityUserManager.FindByNameAsync(username),
                    true
                );
            }

            if (loginResult.Succeeded)
            {
                return new LogInTryoutResult(
                    SuccessLogin(username, viewModel.ReturnUrl).Result,
                    true,
                    await m_identityUserManager.FindByNameAsync(username)
                );
            }

            await m_events.RaiseAsync(new UserLoginFailureEvent(username, "Invalid credentials"));

            ModelState.AddModelError(Translator.Translate("invalid-username-password"));

            return new LogInTryoutResult(View(viewModel));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(
            bool rememberMe, string returnUrl = null, bool showResendMessage = false
        )
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                return Redirect(returnUrl ?? m_returnUrlConfiguration.DefaultRedirectUrl);
            }

            // Ensure the user has gone through the username & password screen first
            var user = await m_signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                ModelState.AddModelError(m_localization.Translate("no-valid-2f-session", "LoginWith2FaViewModel"));
                CacheModelState();
                return RedirectToAction(nameof(Login), new {returnUrl});
            }

            await m_signInManager.GenerateTwoFactorTokenAsync(user, user.TwoFactorProvider);

            var model = new LoginWith2FaViewModel
            {
                RememberMe = rememberMe,
                ReturnUrl = returnUrl,
                Username = user.UserName,
                ShowResendMessage = showResendMessage
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2FaViewModel viewModel)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                return Redirect(viewModel.ReturnUrl ?? m_returnUrlConfiguration.DefaultRedirectUrl);
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await m_signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                ModelState.AddModelError(m_localization.Translate("no-valid-2f-session", "LoginWith2FaViewModel"));
                CacheModelState();
                return RedirectToAction(nameof(Login), new { returnUrl = viewModel.ReturnUrl });
            }

            var result = await m_signInManager.TwoFactorSignInAsync(
                user, viewModel.TwoFactorCode, viewModel.RememberMe, viewModel.RememberMachine, user.TwoFactorProvider
            );

            if (result.Succeeded)
            {
                m_signInManager.DeleteTwoFactorToken(user, user.TwoFactorProvider);

                return await SuccessLogin(user.UserName, viewModel.ReturnUrl);
            }

            ModelState.AddModelError(m_localization.Translate("invalid-authenticator-code"));

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendAuthorizationCode(bool isFormSubmit = false, LoginWith2FaViewModel viewmodel = null)
        {
            if (isFormSubmit && viewmodel != null)
            {
                return RedirectToAction(nameof(LoginWith2fa),
                    new
                    {
                        remeberMe = viewmodel.RememberMe,
                        username = viewmodel.Username,
                        returnUrl = viewmodel.ReturnUrl,
                        ShowResendMessage = true,
                    });
            }

            // Ensure the user has gone through the username & password screen first
            var user = await m_signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException("Unable to load two-factor authentication user.");
            }

            await m_signInManager.GenerateTwoFactorTokenAsync(user, user.TwoFactorProvider);

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CancelLogin(string returnUrl = null)
        {
            var context = await m_interaction.GetAuthorizationContextAsync(returnUrl);

            if (context == null)
            {
                return Redirect(m_returnUrlConfiguration.DefaultRedirectUrl);
            }

            await m_interaction.GrantConsentAsync(context, ConsentResponse.Denied);

            returnUrl = context.Parameters["ReturnUrlOnCancel"] ?? returnUrl;

            return Redirect(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RequestResetPassword(string returnUrl = null)
        {
            var model = new RequestResetPasswordViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await m_identityUserManager.FindByNameOrEmailAsync(viewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(Translator.Translate("user-not-exist", "RequestResetPasswordViewModel"));
                return View(viewModel);
            }

            if (!await m_identityUserManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(Translator.Translate("user-email-not-confirmed", "RequestResetPasswordViewModel"));
                return View(viewModel);
            }

            try
            {
                await m_identityUserManager.SendResetPasswordAsync(Url, user, HttpContext.Request.Protocol);

                return RedirectToAction(nameof(RequestResetPasswordSuccess), new {returnUrl = viewModel.ReturnUrl});
            }
            catch (GenerateResetPasswordTokenException)
            {
                ModelState.AddModelError(Translator.Translate("generate-reset-password-token-exception", "RequestResetPasswordViewModel"));
                return View(viewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RequestResetPasswordSuccess(string returnUrl)
        {
            return View(model: returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string username, string token)
        {
            var user = await m_identityUserManager.FindByNameOrEmailAsync(username);

            var model = new ResetPasswordViewModel
            {
                Username = username,
                PasswordResetToken = token,
                IsVerified = true,
            };

            if (user == null)
            {
                ModelState.AddModelError(Translator.Translate("user-not-exist", "ResetPasswordViewModel"));

                return View(model);
            }

            var verificationResult = await m_identityUserManager.VerifyUserTokenAsync(user,
                m_identityUserManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (!verificationResult)
            {
                model.IsVerified = false;

                ModelState.AddModelError(Translator.Translate("token-not-valid", "ResetPasswordViewModel"));

                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            viewModel.IsVerified = true;

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await m_identityUserManager.FindByNameOrEmailAsync(viewModel.Username);

            if (user == null)
            {
                ModelState.AddModelError(Translator.Translate("user-not-exist", "ResetPasswordViewModel"));
                return View(viewModel);
            }

            var result = await m_identityUserManager.ResetPasswordAsync(user, viewModel.PasswordResetToken, viewModel.Password);

            if (result.Succeeded)
            {
                m_userManager.DeletePasswordResetToken(user.Id);

                return RedirectToAction(nameof(ResetPasswordSuccess));
            }

            viewModel.IsVerified = false;

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordSuccess()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                await m_signInManager.SignOutAsync();
                //await m_loginManager.SignOutUser(HttpContext, int.Parse(User.GetSubjectId()), User.GetDisplayName()); //identity less logout
            }

            var vm = await BuildLoggedOutViewModelAsync(logoutId);

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                var url = Url.Action("Logout", new {logoutId = vm.LogoutId});

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties {RedirectUri = url}, vm.ExternalAuthenticationScheme);
            }

            if (vm.AutomaticRedirectAfterSignOut && !string.IsNullOrEmpty(vm.PostLogoutRedirectUri))
            {
                return View(vm);
            }

            vm.PostLogoutRedirectUri = m_returnUrlConfiguration.DefaultRedirectUrl;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CheckUserAccount(string username, string password)
        {
            if (username == null || password == null)
            {
                return Unauthorized();
            }

            var appUser = await m_identityUserManager.GetUserByUsernameAsync(username);
            var isPasswordValid = await m_identityUserManager.CheckPasswordAsync(appUser, password);

            return isPasswordValid ? (IActionResult) Ok() : Unauthorized();
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await m_interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = true,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId,
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);

                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await m_interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            return Redirect(Url.Action("Index", "Home"));
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string externalProvider, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            // start challenge and roundtrip the return URL and
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl});

            var properties = m_signInManager.ConfigureExternalAuthenticationProperties(externalProvider, redirectUrl);
            properties.SetParameter("returnUrl", returnUrl);

            return Challenge(properties, externalProvider);
        }

        /// <summary>
        /// Initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLoginWithNonce(string externalProvider, string nonce)
        {
            if (!m_nonceManager.IsNonceKeyValid(nonce, NonceTypeEnum.ExternalLogin))
            {
                return Forbid();
            }

            var returnUrl = Url.Action("Index", "Home");

            // start challenge and roundtrip the return URL and
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new {ReturnUrl = returnUrl});

            var properties = m_signInManager.ConfigureExternalAuthenticationProperties(externalProvider, redirectUrl);
            properties.SetParameter("returnUrl", returnUrl);

            return Challenge(properties, externalProvider);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (result?.Succeeded != true)
            {
                throw new ExternalAuthenticationException("External authentication error");
            }

            // lookup our user and external provider info
            var userExternalProvider = await FindUserFromExternalProviderAsync(result);

            if (
                userExternalProvider.User == null
                && m_featureFlagsManager.IsFeatureEnabled(FeatureFlagsEnum.AllowAutoPairWithExternalIdentityProviderThroughAdditionalLogin)
            )
            {
                if (User?.Identity.IsAuthenticated == true)
                {
                    // handle link user with 2FA in operation
                    var userId = int.Parse(User.Identity.GetSubjectId());

                    m_userManager.CreateExternalLogin(
                        userId, userExternalProvider.Provider, userExternalProvider.ProviderUserId
                    );

                    return RedirectToAction(nameof(ExternalLoginCallback), new {returnUrl});
                }

                var dynamicModuleContext = m_dynamicModuleProvider.ModuleContexts.FirstOrDefault(
                    x => x.ModuleConfiguration != null && x.ModuleConfiguration.Name.Equals(userExternalProvider.Provider)
                );

                if (dynamicModuleContext?.LibModuleInfo is IExternalLoginProviderModule externalLoginProviderModule)
                {
                    var externalLoginProviderManager = externalLoginProviderModule.ExternalLoginProviderManager;

                    if (externalLoginProviderManager.CanProvideEmail)
                    {
                        var externalLoginEmail = externalLoginProviderManager.ExtractEmail(userExternalProvider.Claims);

                        if (string.IsNullOrEmpty(externalLoginEmail))
                        {
                            return RedirectToSimplifiedLoginPage(returnUrl, null, ExternalProviderInfoLabelType.MissingEmailFromExternalProvider);
                        }

                        var localUserRequest = m_userManager.GetUserByConfirmedEmail(externalLoginEmail);

                        var localUser = localUserRequest.Result;

                        if (localUser != null)
                        {
                            //TODO pair user using confirmed email pair?

                            return RedirectToSimplifiedLoginPage(returnUrl, externalLoginEmail, ExternalProviderInfoLabelType.UserWithEmailFound);
                        }

                        if (
                            externalLoginProviderManager.CanCreateUser
                            && m_featureFlagsManager.IsFeatureEnabled(FeatureFlagsEnum.AllowRegistrationThroughExternalIdentityProvider)
                        )
                        {
                            userExternalProvider.User = await AutoProvisionUserAsync(
                                userExternalProvider.Provider,
                                userExternalProvider.ProviderUserId,
                                externalLoginProviderManager.MapClaims(userExternalProvider.Claims)
                            );
                        }

                        if (userExternalProvider.User == null)
                        {
                            return RedirectToSimplifiedLoginPage(returnUrl, null, ExternalProviderInfoLabelType.UserCouldNotBeCreated);
                        }
                    }
                }
            }

            if (userExternalProvider.User == null)
            {
                return Redirect(m_returnUrlConfiguration.DefaultFailedExternalLoginUrl);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();

            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForWsFed(result, additionalLocalClaims, localSignInProps);
            ProcessLoginCallbackForSaml2p(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            // we must issue the cookie manually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            var principal = await m_signInManager.CreateUserPrincipalAsync(userExternalProvider.User);
            additionalLocalClaims.AddRange(principal.Claims);

            var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? userExternalProvider.User.Id.ToString();

            await m_events.RaiseAsync(
                new UserLoginSuccessEvent(userExternalProvider.Provider,
                    userExternalProvider.ProviderUserId,
                    userExternalProvider.User.Id.ToString(), name)
            );

            //TODO use signInManager instead of HttpContext?
            await HttpContext.SignInAsync(
                userExternalProvider.User.Id.ToString(), name, userExternalProvider.Provider,
                localSignInProps,
                additionalLocalClaims.ToArray()
            );

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // validate return URL and redirect back to authorization endpoint or a local page

            returnUrl = returnUrl ?? result.Properties.Items["returnUrl"];

            if (m_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(m_returnUrlConfiguration.DefaultRedirectUrl);
        }

        private IActionResult RedirectToSimplifiedLoginPage(string returnUrl, string externalLoginEmail = null,
            ExternalProviderInfoLabelType? externalProviderInfoLabelType = null)
        {
            var viewModel = ViewModelFactory.GetLoginViewModel(returnUrl);

            viewModel.Action = nameof(ExternalLoginCallback);
            viewModel.LinkExternalIdentity = true;
            viewModel.ExternalProviders = null;
            viewModel.ExternalProviderInfoLabelType = externalProviderInfoLabelType;

            if (!string.IsNullOrEmpty(externalLoginEmail))
            {
                if (viewModel.Input == null)
                {
                    viewModel.Input = new LoginViewModel.LoginFormModel();
                }

                viewModel.Input.Username = externalLoginEmail;
            }

            return View(nameof(Login), viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback([Required] LoginViewModel viewModel)
        {
            var logInTryoutResult = await PerformLogInTryout(viewModel);

            if (logInTryoutResult.Success || User?.Identity.IsAuthenticated == true)
            {
                var userId = logInTryoutResult.Success ? logInTryoutResult.User.Id : int.Parse(User.Identity.GetSubjectId());

                var authorizationResult = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

                if (authorizationResult.Succeeded)
                {
                    var userExternalProvider = await FindUserFromExternalProviderAsync(authorizationResult);

                    m_userManager.CreateExternalLogin(
                        userId, userExternalProvider.Provider, userExternalProvider.ProviderUserId
                    );
                }
                else
                {
                    ModelState.AddModelError(Translator.Translate("failed-login-through-external-provider"));

                    viewModel.Action = nameof(ExternalLoginCallback);
                    viewModel.LinkExternalIdentity = true;
                    viewModel.ExternalProviders = null;

                    return View(nameof(Login), viewModel);
                }
            }
            else if (logInTryoutResult.SecondFactorRequired)
            {
                await m_signInManager.GenerateTwoFactorTokenAsync(logInTryoutResult.User, logInTryoutResult.User.TwoFactorProvider);

                var model = new LoginWith2FaViewModel
                {
                    RememberMe = viewModel.Input.RememberLogin,
                    ReturnUrl = Url.Action(nameof(ExternalLoginCallback), new {viewModel.ReturnUrl}),
                    Username = logInTryoutResult.User.UserName,
                    ShowResendMessage = false
                };

                return View(nameof(LoginWith2fa), model);
            }

            if (logInTryoutResult.ActionResult is ViewResult)
            {
                viewModel.Action = nameof(ExternalLoginCallback);
                viewModel.LinkExternalIdentity = true;
                viewModel.ExternalProviders = null;

                return View(nameof(Login), viewModel);
            }

            return logInTryoutResult.ActionResult;
        }

        /// <summary>
        /// Checks if user is logged in and returns JavaScript that contains redirect function for starting login process or empty file when user is not logged in.
        /// JavaScript is loaded from file wwwroot\js\ridics.sso.js
        /// </summary>
        /// <returns>JavaScript whether user is logged in</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckLogin()
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                try
                {
                    var javaScriptCode = System.IO.File.ReadAllText(SsoJavaScriptPath);

                    return JavaScript(javaScriptCode);
                }
                catch (IOException)
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                        Logger.LogError($"Can't read JavaScript file required for SSO login: {SsoJavaScriptPath}");

                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return JavaScript(string.Empty);
        }

        private async Task<UserExternalProvider> FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new ExternalAuthenticationException("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            //TODO discover why Items not contains "scheme" key
            var provider = result.Properties.Items[".AuthScheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await m_identityUserManager.FindByLoginAsync(provider, providerUserId);

            var userExternalProvider = new UserExternalProvider
            {
                User = user,
                Provider = provider,
                ProviderUserId = providerUserId,
                Claims = claims
            };

            return userExternalProvider;
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);

            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue(OidcConstants.ResponseTypes.IdToken);

            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[]
                {
                    new AuthenticationToken
                    {
                        Name = OidcConstants.ResponseTypes.IdToken,
                        Value = idToken
                    }
                });
            }
        }

        private void ProcessLoginCallbackForWsFed(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        private void ProcessLoginCallbackForSaml2p(AuthenticateResult externalResult, List<Claim> localClaims,
            AuthenticationProperties localSignInProps)
        {
        }

        //HACK investigate correctness of mapping external account

        private async Task<ApplicationUser> AutoProvisionUserAsync(
            string provider, string providerUserId, ExternalLoginProviderUserModel externalLoginProvider
        )
        {
            var userId = m_userManager.CreateUserByExternalProvider(provider, externalLoginProvider);

            if (userId.Succeeded)
            {
                m_userManager.CreateExternalLogin(userId.Result, provider, providerUserId);

                m_userManager.AddRoleToUser(userId.Result, RoleNames.RegisteredUser);

                var user = await m_identityUserManager.FindByIdAsync(userId.Result.ToString());

                return user;
            }

            return null;
        }

        private async Task<IActionResult> SuccessLogin(string username, string returnUrl)
        {
            var userResult = m_userManager.GetUserByUsername(username);

            if (!userResult.HasError)
            {
                var user = userResult.Result;

                await m_events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.Id.ToString(), user.Username));

                if (m_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return Redirect(m_returnUrlConfiguration.DefaultRedirectUrl);
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(m_returnUrlConfiguration.DefaultRedirectUrl);
        }

        private ContentResult JavaScript(string content)
        {
            return new ContentResult
            {
                Content = content,
                ContentType = ContentType.ApplicationJavascript,
            };
        }
    }

    public enum ExternalProviderInfoLabelType
    {
        UserWithEmailFound,
        UserCouldNotBeCreated,
        MissingEmailFromExternalProvider
    }
}
