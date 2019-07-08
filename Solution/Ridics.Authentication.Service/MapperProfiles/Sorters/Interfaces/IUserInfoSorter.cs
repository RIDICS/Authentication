using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IUserInfoSorter
    {
        IList<UserInfoModel> SortUserInfos(IList<UserInfoModel> users);
    }
}