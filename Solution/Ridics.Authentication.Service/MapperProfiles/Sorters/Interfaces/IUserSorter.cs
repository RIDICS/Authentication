using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.Users;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IUserSorter
    {
        List<UserViewModel> SortUsers(List<UserViewModel> users);
    }
}