using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IUserDataSorter
    {
        IList<UserDataModel> SortUserData(IList<UserDataModel> userData);
    }
}