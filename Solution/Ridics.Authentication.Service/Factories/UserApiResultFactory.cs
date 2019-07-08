using System;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities;
using Ridics.Authentication.Service.Models.API.UserActivation;
using Ridics.Core.Shared.Types;

namespace Ridics.Authentication.Service.Factories
{
    public class UserApiResultFactory
    {
        public UserActivationResponse CreateResultForLastName(UserModel user)
        {
            var lastNameData = user.UserData.First(x => x.UserDataType.DataTypeValue == UserDataTypes.LastName);
            var masterUserIdData = user.UserData.First(x => x.UserDataType.DataTypeValue == UserDataTypes.MasterUserId);

            var result = new UserActivationResponse
            {
                Activated = lastNameData.LevelOfAssurance.Level >= (int)LevelsOfAssurance.UserDataMinLoaToBeActivatedForExternalService,
                ActivationTime = lastNameData.ActiveFrom,
                Muid = null
            };
            if (result.Activated)
            {
                result.Muid = Guid.Parse(masterUserIdData.Value);
            }

            return result;
        }
    }
}
