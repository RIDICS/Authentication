using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class UserInfoByUsernameSorter : IUserInfoSorter
    {
        public IList<UserInfoModel> SortUserInfos(IList<UserInfoModel> users)
        {
            users = users.OrderBy(x => x.Username).ToList();

            return users;
        }
    }
}