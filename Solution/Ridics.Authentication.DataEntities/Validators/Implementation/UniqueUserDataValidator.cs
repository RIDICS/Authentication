using System.Linq;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Proxies;
using Ridics.Authentication.DataEntities.Validators.Interface;

namespace Ridics.Authentication.DataEntities.Validators.Implementation
{
    public class UniqueUserDataValidator : IUserDataValidator
    {
        private readonly UserDataVersioningProxy m_userDataRepository;

        public UniqueUserDataValidator(UserDataVersioningProxy userDataRepository)
        {
            m_userDataRepository = userDataRepository;
        }

        public string Type => "unique";

        /// <summary>
        /// Checks if user data (value and type) already exists in database, if yes throws exception
        /// </summary>
        /// <param name="userData">User data to be checked</param>
        /// <param name="validationOptions">Options for validation</param>
        /// <exception cref="UserDataAlreadyExistsException">Throws exception if user data exists in database</exception>
        public void Validate(UserDataEntity userData, ValidationOptions validationOptions)
        {
            var value = userData.Value;
            var type = userData.UserDataType.DataTypeValue;

            var userDataEntity = m_userDataRepository.FindUserDataWithLoaGreaterThan(value, type, validationOptions.MinLevelOfAssuranceToValidateAgainst).FirstOrDefault();//UserData has to be unique with specified MinLevelOfAssuranceToValidateAgainst so FindUserDataWithLoaGreaterThan should return only one result 

            if (userDataEntity != null && (!validationOptions.UserId.HasValue || validationOptions.UserId.Value != userDataEntity.User.Id))
            {
                throw new UserDataAlreadyExistsException(value, type);
            }
        }
    }
}