using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IContactSorter
    {
        IList<UserContactModel> SortContacts(IList<UserContactModel> contacts);
    }
}