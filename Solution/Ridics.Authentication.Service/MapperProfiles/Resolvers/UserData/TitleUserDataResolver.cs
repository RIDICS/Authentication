using System;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class TitleUserDataResolver<T> : IValueResolver<UserModel, T, UserAddressingWays>
    {
        private readonly UserDataStringValueResolver<T> m_userDataStringValueResolver;

        public TitleUserDataResolver(string userDataType)
        {
            m_userDataStringValueResolver = new UserDataStringValueResolver<T>(userDataType);
        }

        public UserAddressingWays Resolve(UserModel source, T destination, UserAddressingWays destMember, ResolutionContext context)
        {
            var stringUserTitle = m_userDataStringValueResolver.Resolve(source, destination, null, context);

            if (stringUserTitle == null)
            {
                return UserAddressingWays.None;
            }

            return Enum.TryParse<UserAddressingWays>(stringUserTitle, out var userTitle) ? userTitle : UserAddressingWays.None;
        }
    }
}