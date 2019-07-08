using System.Collections.Generic;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Comparer
{
    public class UserDataTypeEqualityComparer : IEqualityComparer<UserDataEntity>
    {
        public bool Equals(UserDataEntity x, UserDataEntity y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (x != null && y != null)
            {
                return x.UserDataType.DataTypeValue == y.UserDataType.DataTypeValue;
            }

            return false;
        }

        public int GetHashCode(UserDataEntity userData)
        {
            return userData.UserDataType.DataTypeValue.GetHashCode();
        }
    }
}