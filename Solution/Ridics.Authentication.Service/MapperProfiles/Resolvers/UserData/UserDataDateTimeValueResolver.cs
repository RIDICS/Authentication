using System;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Core.Utils.Helpers;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class UserDataDateTimeValueResolver<T> : IValueResolver<UserModel, T, DateTime>
    {
        private readonly IFormatProvider m_formatProvider;
        private readonly UserDataStringValueResolver<T> m_userDataStringValueResolver;

        public UserDataDateTimeValueResolver(string userDataType, IFormatProvider formatProvider = null)
        {
            m_formatProvider = formatProvider;
            m_userDataStringValueResolver = new UserDataStringValueResolver<T>(userDataType);
        }

        public DateTime Resolve(UserModel source, T destination, DateTime destMember, ResolutionContext context)
        {
            var stringDateTime = m_userDataStringValueResolver.Resolve(source, destination, null, context);

            if (stringDateTime == null)
            {
                return default(DateTime);
            }

            try
            {
                var dateTime = DateTimeStringMapper.StringToDate(stringDateTime);
                return dateTime;
            }
            catch (ArgumentException)
            {
                return m_formatProvider != null ? DateTime.Parse(stringDateTime, m_formatProvider) : default(DateTime);
            }
        }
    }
}