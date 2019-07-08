using System.Collections.Generic;
using Ridics.Authentication.Core.Models.DataResult;

namespace Ridics.Authentication.Core.UserData
{
    public static class UserDataTypeErrorCodeHelper
    {
        private static readonly IDictionary<string, string> m_codesForUniqueUserDataTypeException = new Dictionary<string, string>
        {
            // Possibly add UserData which must be unique for user, e.g.
            //{CustomUserDataTypes.FullName, DataResultErrorCode.FullNameNotUnique},
        };

        public static string GetCodeForUniqueUserDataTypeException(string userDataType)
        {
            return m_codesForUniqueUserDataTypeException.TryGetValue(userDataType, out var code) ? code : DataResultErrorCode.GenericError;
        }
    }
}