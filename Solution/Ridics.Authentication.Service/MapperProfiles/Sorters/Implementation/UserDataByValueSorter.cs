using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class UserDataByValueSorter : IUserDataSorter
    {
        public IList<UserDataModel> SortUserData(IList<UserDataModel> userData)
        {
            userData = userData.OrderBy(x => x.Value).ToList();

            return userData;
        }
    }
}