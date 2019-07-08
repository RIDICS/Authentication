using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts
{
    public class UserContactLevelOfAssuranceResolver<TDestType> : IValueResolver<UserModel, TDestType, ContactLevelOfAssuranceEnum>
    {
        private readonly ContactTypeEnumModel m_contactType;

        public UserContactLevelOfAssuranceResolver(ContactTypeEnumModel contactType)
        {
            m_contactType = contactType;
        }

        public ContactLevelOfAssuranceEnum Resolve(UserModel source, TDestType destination, ContactLevelOfAssuranceEnum destMember, ResolutionContext context)
        {
            var userContact = source.UserContacts.FirstOrDefault(x => x.Type == m_contactType);
            if (userContact != null)
            {
                return (ContactLevelOfAssuranceEnum)userContact.LevelOfAssurance.Level;
            }

            return ContactLevelOfAssuranceEnum.Low;
        }
    }
}