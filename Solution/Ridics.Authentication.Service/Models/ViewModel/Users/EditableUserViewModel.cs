using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.Models.ViewModel.Users
{
    public class EditableUserViewModel<T> where T : UserViewModel
    {
        public T UserViewModel { get; set; }

        public IList<SelectableViewModel<RoleViewModel>> SelectableRoles { get; set; }

        public IList<ExternalLoginProviderViewModel> AvailableExternalLoginProviders { get; set; }

        public string ReturnUrl { get; set; }
    }
}