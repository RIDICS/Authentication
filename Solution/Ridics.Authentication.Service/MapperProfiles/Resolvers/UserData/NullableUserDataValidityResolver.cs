using System;
using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class NullableUserDataValidityResolver<T> : IValueResolver<UserModel, T, DateTime?>
    {
        private readonly string m_userDataType;

        public NullableUserDataValidityResolver(string userDataType)
        {
            m_userDataType = userDataType;
        }

        public virtual DateTime? Resolve(UserModel source, T destination, DateTime? destMember, ResolutionContext context)
        {
            var userData = source.UserData.FirstOrDefault(x => x.UserDataType.DataTypeValue == m_userDataType);

            return userData?.ActiveTo;
        }
    }
}