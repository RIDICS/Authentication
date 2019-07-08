using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Users;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class UserByLastAndFirstNameSorter : IUserSorter
    {
        public List<UserViewModel> SortUsers(List<UserViewModel> users)
        {
            users = users.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ToList();

            return users;
        }
    }
}