using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Utils;
using Ridics.Authentication.DataEntities.Validators.Config;
using Ridics.Authentication.DataEntities.Validators.Interface;

namespace Ridics.Authentication.DataEntities.Validators
{
    public class UserDataValidatorManager : IUserDataValidatorManager
    {
        private readonly IDictionary<string, IUserDataValidator> m_dataValidatorsDic;
        private readonly ValidatorConfig m_validatorConfig;
        private readonly UserDataStructureConvertor m_userDataStructureConvertor;

        public UserDataValidatorManager(IEnumerable<IUserDataValidator> dataValidators, ValidatorConfig validatorConfig, UserDataStructureConvertor userDataStructureConvertor)
        {
            m_validatorConfig = validatorConfig;
            m_userDataStructureConvertor = userDataStructureConvertor;
            m_dataValidatorsDic = dataValidators.ToDictionary(x => x.Type);
        }

        /// <summary>
        /// Validates list of user data
        /// </summary>
        /// <param name="userDataList">User data list to be validated</param>
        /// <param name="userId">User id to whom user data belong</param>
        public void ValidateUserData(
            IEnumerable<UserDataEntity> userDataList,
            int? userId = null
        )
        { 
            if (userDataList == null) return;

            var flatUserDataList = m_userDataStructureConvertor.GetFlatUserDataList(userDataList);

            var validationOptions = new ValidationOptions
            {
                UserId = userId,
                MinLevelOfAssuranceToValidateAgainst = LevelsOfAssurance.UserDataLoaToBeGreaterThanForUniqueValidation,
            };

            foreach (var userData in flatUserDataList)
            {
                var validatorTypes = GetValidatorTypes(userData.UserDataType.DataTypeValue);

                foreach (var validatorType in validatorTypes)
                {
                    m_dataValidatorsDic[validatorType].Validate(userData, validationOptions);
                }
            }
        }

        /// <summary>
        /// For user data type returns list of validator names that should validate the specified user data type
        /// </summary>
        /// <param name="type">Type of user data</param>
        /// <returns>List of validators names</returns>
        private IEnumerable<string> GetValidatorTypes(string type)
        {
            var list = new List<string>();

            var dict = m_validatorConfig.ValidatorsToValidatedDictionary;

            if (dict == null)
            {
                return list;
            }

            foreach (var key in dict.Keys)
            {
                if (dict[key].Contains(type)) list.Add(key);
            }

            return list;
        }
    }
}
