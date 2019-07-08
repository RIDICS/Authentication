using System.Collections.Generic;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Validators
{
    public interface IUserDataValidatorManager
    {
        void ValidateUserData(
            IEnumerable<UserDataEntity> userDataList,
            int? userId = null
        );
    }
}
