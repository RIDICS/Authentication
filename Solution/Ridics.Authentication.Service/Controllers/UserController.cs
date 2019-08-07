using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using IdentityModel;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authentication.Identity.Stores;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Constants;
using Ridics.Authentication.Service.Extensions;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Models.ViewModel;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers
{
    //[Authorize(Policy = PolicyNames.UserPolicy)]

    public class UserController : AuthControllerBase<UserController>
    {
        private readonly IAuthorizationService m_authorizationService;
        private readonly UserManager m_usersManager;
        private readonly IdentityUserManager m_identityUserManager;
        private readonly IdentitySignInManager m_signInManager;
        private readonly ClaimManager m_claimManager;
        private readonly UserStore m_userStore;
        private readonly TwoFactorValidator m_twoFactorValidator;
        private readonly NonceManager m_nonceManager;
        private readonly IMapper m_mapper;

        public UserController(
            UserManager usersManager, IdentityUserManager identityUserManager,
            IAuthorizationService authorizationService, IdentitySignInManager signInManager,
            ClaimManager claimManager, TwoFactorValidator twoFactorValidator,
            UserStore userStore, NonceManager nonceManager,
            IMapper mapper
        )
        {
            m_authorizationService = authorizationService;
            m_usersManager = usersManager;
            m_identityUserManager = identityUserManager;

            m_signInManager = signInManager;

            m_twoFactorValidator = twoFactorValidator;
            m_claimManager = claimManager;
            m_userStore = userStore;
            m_nonceManager = nonceManager;
            m_mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult Index(int start = PaginationConstants.StartItemIndex,
            int count = PaginationConstants.ItemsOnPage, string searchByName = null, bool partial = false)
        {
            LoadCachedModelState();
            var usersResult = m_usersManager.FindUsers(start, count, searchByName);
            var itemsCountResult = m_usersManager.GetUsersCount(searchByName);

            if (usersResult.HasError)
            {
                ModelState.AddModelError(usersResult.Error.Message);
                return View();
            }

            if (itemsCountResult.HasError)
            {
                ModelState.AddModelError(itemsCountResult.Error.Message);
                return View();
            }

            ViewData["search"] = searchByName;

            var userViewModels = m_mapper.Map<IList<UserViewModel>>(usersResult.Result);
            var itemsCount = itemsCountResult.Result;

            var vm = ViewModelFactory.GetListViewModel(userViewModels,
                Translator.Translate("delete-user-confirm-dialog-title"),
                Translator.Translate("delete-user-confirm-dialog-message"),
                itemsCount, count);

            if (partial)
            {
                return PartialView("_UserList", vm);
            }

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult View(int id)
        {
            var authorized =
                m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, id); //TODO consider authorization for view user

            if (!authorized.Result)
            {
                return Forbid();
            }

            var result = m_usersManager.GetUserById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var viewModel = Mapper.Map<UserViewModel>(result.Result);

            var vm = ViewModelFactory.GetViewModel(viewModel,
                Translator.Translate("delete-user-confirm-dialog-title"),
                Translator.Translate("delete-user-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult Create()
        {
            var viewModel = ViewModelBuilder.BuildCreateUserViewModel(ModelState);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult Create(EditableUserViewModel<UserWithPasswordViewModel> editableViewModel)
        {
            if (ModelState.IsValid)
            {
                var userViewModel = editableViewModel.UserViewModel as UserViewModel;


                var userModel = m_mapper.Map<UserModel>(userViewModel);
                var applicationUser = m_mapper.Map<ApplicationUser>(userModel);

                var result = m_identityUserManager
                    .CreateAsync(applicationUser, editableViewModel.UserViewModel.Password).Result;

                if (result.Succeeded)
                {
                    var roleResult = m_identityUserManager.AddToRolesAsync(applicationUser,
                        editableViewModel.SelectableRoles?.Where(x => x.IsSelected).Select(x => x.Item.Name).ToList()).Result;

                    if (roleResult.Succeeded)
                    {
                        return RedirectToAction(nameof(View), new {id = applicationUser.Id});
                    }

                    ModelState.AddModelError(
                        roleResult.Errors.FirstOrDefault()?.Description); //TODO translate by code
                }

                ModelState.AddModelError(
                    result.Errors.FirstOrDefault()?.Description); //TODO translate by code
            }

            var viewModel = ViewModelBuilder.BuildCreateUserViewModel(ModelState, editableViewModel.UserViewModel);

            return View(viewModel);
        }

        [HttpGet]
        [Route("[controller]/{id}/[action]")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public async Task<IActionResult> Edit(int id)
        {
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, id);

            if (!authorized.Result)
            {
                return Forbid();
            }

            var result = m_usersManager.GetUserById(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var userViewmodel = m_mapper.Map<UserViewModel>(result.Result);

            var viewModel = await ViewModelBuilder.BuildEditableUserViewModelAsync(ModelState, userViewmodel);

            return View(viewModel);
        }

        [HttpPost]
        [HttpPut]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{id}/[action]")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public async Task<IActionResult> Edit(int id, EditableUserViewModel<UserViewModel> editableViewModel)
        {
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, id);

            if (!authorized.Result)
            {
                return Forbid();
            }

            var userViewModel = editableViewModel.UserViewModel;


            var twoFactorCheckResult = await m_twoFactorValidator.CheckTwoFactorIsValidOrNotEnabledAsync(id, userViewModel);

            if (!twoFactorCheckResult.IsSuccessful)
            {
                ModelState.AddModelError("", twoFactorCheckResult.Message);
            }

            var userModel = m_mapper.Map<UserModel>(userViewModel);
            var appUser = m_mapper.Map<ApplicationUser>(userModel);

            var result = m_identityUserManager.UpdateAsync(id, appUser).Result;

            if (result.Succeeded)
            {
                if (int.TryParse(User.FindFirst(JwtClaimTypes.Subject).Value, out var value) && value == id)
                {
                    await m_signInManager.ReloginUserAsync(id, false); //HACK check for persistent login
                }

                return RedirectToAction(nameof(View), new {id});
            }


            ModelState.AddModelError(result.Errors.FirstOrDefault()?.Description); //TODO translate by code


            var viewModel = await ViewModelBuilder.BuildEditableUserViewModelAsync(ModelState, userViewModel);

            return View(viewModel);
        }


        [HttpPost]
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        [Route("[controller]/{id}/[action]")]
        public IActionResult Delete(int id)
        {
            var result = m_usersManager.DeleteUserWithId(id);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("[controller]/{userId}/[action]")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult Claims(int userId)
        {
            LoadCachedModelState();
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, userId);

            if (!authorized.Result)
            {
                return Forbid();
            }

            var result = m_claimManager.GetUserClaimsByUserId(userId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                return View();
            }

            var userClaims = m_mapper.Map<IList<ClaimViewModel>>(result.Result);

            var vm = ViewModelFactory.GetUserClaimsViewModel(userClaims, userId,
                Translator.Translate("remove-user-claim-confirm-dialog-title"),
                Translator.Translate("remove-user-claim-confirm-dialog-message"));

            return View(vm);
        }

        [HttpGet]
        [Route("[controller]/{userId}/[action]/")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult AddClaim(int userId)
        {
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, userId);

            if (!authorized.Result)
            {
                return Forbid();
            }

            var viewModel = ViewModelBuilder.BuildAddClaimViewModel(ModelState, userId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{userId}/[action]/")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult AddClaim(int userId, EditableClaimViewModel claim)
        {
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, userId);
            if (!authorized.Result)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                var viewModel = ViewModelBuilder.BuildAddClaimViewModel(ModelState, userId);
                return View(viewModel);
            }

            var claimModel = m_mapper.Map<ClaimModel>(claim);
            var result = m_claimManager.AddClaimToUser(userId, claimModel, claim.SelectedClaimType);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildAddClaimViewModel(ModelState, userId);
                return View(viewModel);
            }


            return RedirectToAction(nameof(Claims), new
            {
                userId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/{userId}/[action]/{typeId}")]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        public IActionResult RemoveClaim(int userId, int typeId)
        {
            var authorized = m_authorizationService.AuthorizationForUserEditAsync(User, RoleNames.Admin, userId);

            if (!authorized.Result)
            {
                return Forbid();
            }

            var result = m_claimManager.RemoveClaimFromUser(userId, typeId);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);
                CacheModelState();
            }


            return RedirectToAction(nameof(Claims), new
            {
                userId
            });
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult Roles(int userId)
        {
            var viewModel = ViewModelBuilder.BuildUserRolesViewModel(ModelState, userId);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult Roles(int userId, List<SelectableViewModel<RoleViewModel>> selectableRoles)
        {
            var rolesIds = GetSelectedItems(selectableRoles).Select(x => x.Id);

            var result = m_usersManager.AssignRolesToUser(userId, rolesIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildUserRolesViewModel(ModelState, userId);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id = userId});
        }

        [HttpGet]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult ResourcePermissions(int userId)
        {
            var viewModel = ViewModelBuilder.BuildUserResourcePermissionsViewModel(ModelState, userId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult ResourcePermissions(int userId, List<SelectableViewModel<ResourcePermissionViewModel>> selectablePermissions)
        {
            var permissionsIds = GetSelectedItems(selectablePermissions).Select(x => x.Id);

            var result = m_usersManager.AssignResourcePermissionsToUser(userId, permissionsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildUserResourcePermissionsViewModel(ModelState, userId);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id = userId});
        }

        [HttpGet]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult ResourcePermissionTypeActions(int userId)
        {
            var viewModel = ViewModelBuilder.BuildUserResourcePermissionTypeActionsViewModel(ModelState, userId);

            return View(viewModel);
        }

        [HttpPost]
        [Route("[controller]/{userId}/[action]")]
        public IActionResult ResourcePermissionTypeActions(int userId,
            List<SelectableViewModel<ResourcePermissionTypeActionViewModel>> selectablePermissions)
        {
            var permissionsIds = GetSelectedItems(selectablePermissions).Select(x => x.Id);

            var result = m_usersManager.AssignResourcePermissionTypeActionsToUser(userId, permissionsIds);

            if (result.HasError)
            {
                ModelState.AddModelError(result.Error.Message);

                var viewModel = ViewModelBuilder.BuildUserResourcePermissionTypeActionsViewModel(ModelState, userId);

                return View(viewModel);
            }

            return RedirectToAction(nameof(View), new {id = userId});
        }

        [HttpPost]
        public IActionResult AddExternalLogin(string externalProvider, string returnUrl)
        {
            returnUrl = returnUrl ?? GetDefaultRedirectUrlForUser(int.Parse(User.Identity.GetSubjectId()));

            // start challenge and roundtrip the return URL and
            var redirectUrl = Url.Action("LinkLoginCallback", new {ReturnUrl = returnUrl});

            var properties = m_signInManager.ConfigureExternalAuthenticationProperties(
                externalProvider, redirectUrl,
                User.Identity.GetSubjectId()
            );

            return Challenge(properties, externalProvider);
        }

        [HttpGet]
        public IActionResult AddExternalLoginWithNonce(
            int userId,
            string externalProvider,
            string nonce,
            string returnUrl
        )
        {
            //TODO validate returnUrl also?
            if (!m_nonceManager.IsNonceKeyValid(nonce, userId, NonceTypeEnum.AddExternalLogin))
            {
                return Forbid();
            }

            returnUrl = returnUrl ?? GetDefaultRedirectUrlForUser(userId);

            // start challenge and roundtrip the return URL and
            var redirectUrl = Url.Action("LinkLoginCallback", new {ReturnUrl = returnUrl});

            var properties = m_signInManager.ConfigureExternalAuthenticationProperties(
                externalProvider, redirectUrl,
                userId.ToString()
            );

            return Challenge(properties, externalProvider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback(string returnUrl)
        {
            var userModelResult = m_usersManager.GetUserById(int.Parse(User.Identity.GetSubjectId()));

            if (userModelResult.HasError)
            {
                ModelState.AddModelError(userModelResult.Error.Message);

                return !returnUrl.IsNullOrEmpty() ? GetRedirectReturnAction(returnUrl, userModelResult.Error.Code) : View("Error");
            }

            var user = m_mapper.Map<ApplicationUser>(userModelResult.Result);

            if (user == null)
            {
                return !returnUrl.IsNullOrEmpty() ? GetRedirectReturnAction(returnUrl) : View("Error");
            }

            var info = await m_signInManager.GetExternalLoginInfoAsync(
                await m_userStore.GetUserIdAsync(user, CancellationToken.None)
            );

            if (info == null)
            {
                return !returnUrl.IsNullOrEmpty()
                    ? GetRedirectReturnAction(returnUrl)
                    : RedirectToAction(nameof(Edit), new {id = User.Identity.GetSubjectId()});
            }

            var userModel = m_mapper.Map<UserModel>(user);

            var resultCreateLink = m_usersManager.CreateExternalLogin(
                userModel.Id, info.LoginProvider, info.ProviderKey
            );

            if (resultCreateLink.HasError)
            {
                return !returnUrl.IsNullOrEmpty() ? GetRedirectReturnAction(returnUrl, resultCreateLink.Error.Code) : View("Error");
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        public IActionResult DisconnectExternalLogin(int externalLoginId, string returnUrl)
        {
            returnUrl = returnUrl ?? GetDefaultRedirectUrlForUser(int.Parse(User.Identity.GetSubjectId()));

            var userModel = m_usersManager.GetUserById(int.Parse(User.Identity.GetSubjectId()));

            if (userModel.HasError)
            {
                ModelState.AddModelError(userModel.Error.Message);

                return View("Error");
            }

            var user = m_mapper.Map<ApplicationUser>(userModel.Result);

            if (user == null)
            {
                return View("Error");
            }

            var resultDeleteLink = m_usersManager.DeleteExternalLoginByUser(user.Id, externalLoginId);

            if (resultDeleteLink.HasError)
            {
                return View("Error");
            }

            return Redirect(returnUrl);
        }

        private string GetDefaultRedirectUrlForUser(int id)
        {
            return Url.Action(
                "Edit", "User", new {id = id},
                HttpContext.Request.Scheme,
                HttpContext.Request.Host.ToString()
            );
        }

        private IActionResult GetRedirectReturnAction(string returnUrl, string error = null)
        {
            if (returnUrl.IsNullOrEmpty())
            {
                return null;
            }

            var uriBuilder = new UriBuilder(returnUrl);

            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["error"] = string.IsNullOrEmpty(error)
                ? string.Join('&', ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))
                : error;

            uriBuilder.Query = query.ToString();

            return Redirect(uriBuilder.ToString());
        }
    }
}
