using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class UserDataStringValueResolver<T> : IValueResolver<UserModel, T, string>
    {
        private readonly string m_userDataType;

        public UserDataStringValueResolver(string userDataType)
        {
            m_userDataType = userDataType;
        }   

        public string Resolve(UserModel source, T destination, string destMember, ResolutionContext context)
        {
            var userData = source.UserData.FirstOrDefault(x => x.UserDataType.DataTypeValue == m_userDataType);

            return userData?.Value;
        }
    }
}