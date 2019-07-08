using System.Linq;
using Ridics.Authentication.DataEntities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Proxies;

namespace Ridics.Authentication.Core.Utils.Validator
{
    public class UniqueContactValidator
    {
        private readonly UserContactVersioningProxy m_userContactRepository;

        public UniqueContactValidator(UserContactVersioningProxy userContactRepository)
        {
            m_userContactRepository = userContactRepository;
        }
        
        /// <summary>
        /// Checks if contact is not already in database
        /// </summary>
        /// <param name="type">Contact type</param>
        /// <param name="value">Contact value</param>
        /// <param name="userId">User id to whom the contact belongs</param>
        /// <returns>True if contact is not in database, false otherwise</returns>
        public bool Validate(ContactTypeEnum type, string value, int? userId)
        {
            var contact = m_userContactRepository.GetUserContactsWithLoaGreaterThan(value, type, LevelsOfAssurance.ContactLoaToBeGreaterThanForUniqueValidation).FirstOrDefault();//UserContact has to be unique with specified MinLevelOfAssuranceToValidateAgainst so GetUserContactsWithLoaGreaterThan should return only one result 
            
            return contact == null || (userId.HasValue && userId.Value == contact.User.Id);
        }
    }
}