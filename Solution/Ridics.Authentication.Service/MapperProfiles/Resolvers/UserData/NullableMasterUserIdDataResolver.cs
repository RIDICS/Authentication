using System;
using AutoMapper;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class NullableMasterUserIdDataResolver<T> : IValueResolver<UserModel, T, Guid?>
    {
        private readonly UserDataStringValueResolver<T> m_userDataStringValueResolver;

        public NullableMasterUserIdDataResolver(string userDataType)
        {
            m_userDataStringValueResolver = new UserDataStringValueResolver<T>(userDataType);
        }

        public Guid? Resolve(UserModel source, T destination, Guid? destMember, ResolutionContext context)
        {
            var stringMasterUserId = m_userDataStringValueResolver.Resolve(source, destination, null, context);

            if (stringMasterUserId == null)
            {
                return null;
            }

            return Guid.Parse(stringMasterUserId);
        }
    }
}