using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ContactByValueSorter : IContactSorter
    {
        public IList<UserContactModel> SortContacts(IList<UserContactModel> contacts)
        {
            contacts = contacts.OrderBy(x => x.Value).ToList();

            return contacts;
        }
    }
}