using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataEntities;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts
{
    public class UserContactConfirmResolver<TDestType> : IValueResolver<UserModel, TDestType, bool>
    {
        private readonly ContactTypeEnumModel m_contactType;

        public UserContactConfirmResolver(ContactTypeEnumModel contactType)
        {
            m_contactType = contactType;
        }

        public bool Resolve(UserModel source, TDestType destination, bool destMember, ResolutionContext context)
        {
            var userContact = source.UserContacts.FirstOrDefault(x => x.Type == m_contactType);
            if (userContact != null)
            {
                return userContact.LevelOfAssurance.Level >= (int)LevelsOfAssurance.ContactMinLoaToBeConfirmed;
            }

            return false;
        }
    }
}