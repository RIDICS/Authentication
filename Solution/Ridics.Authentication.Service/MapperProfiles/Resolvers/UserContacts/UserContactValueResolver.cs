using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts
{
    public class UserContactValueResolver<TDestType> : IValueResolver<UserModel, TDestType, string>
    {
        private readonly ContactTypeEnumModel m_contactType;

        public UserContactValueResolver(ContactTypeEnumModel contactType)
        {
            m_contactType = contactType;
        }

        public string Resolve(UserModel source, TDestType destination, string destMember, ResolutionContext context)
        {
            var userContact = source.UserContacts.FirstOrDefault(x => x.Type == m_contactType);
            return userContact?.Value;
        }
    }
}