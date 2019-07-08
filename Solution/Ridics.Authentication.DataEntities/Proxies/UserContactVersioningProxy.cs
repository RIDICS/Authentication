using System;
using System.Collections.Generic;
using Ridics.Authentication.DataEntities.Comparer;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.Shared.Providers;

namespace Ridics.Authentication.DataEntities.Proxies
{
    public class UserContactVersioningProxy
    {
        private readonly UserContactRepository m_userContactRepository;
        private readonly IDateTimeProvider m_dateTimeProvider;
        private readonly UserContactEqualityComparer m_userContactEqualityComparer;

        public UserContactVersioningProxy(UserContactRepository userContactRepository, UserContactEqualityComparer userContactEqualityComparer, IDateTimeProvider dateTimeProvider)
        {
            m_userContactRepository = userContactRepository;
            m_dateTimeProvider = dateTimeProvider;
            m_userContactEqualityComparer = userContactEqualityComparer;
        }

        public UserContactEntity GetUserContact(int userId, ContactTypeEnum contactTypeEnum)
        {
            return m_userContactRepository.GetActualVersionOfUserContact(userId, contactTypeEnum);
        }

        public IList<UserContactEntity> GetUserContacts(int userEntityId)
        {
            return m_userContactRepository.GetActualVersionsOfUserContacts(userEntityId);
        }

        public void UpdateUserContacts(IEnumerable<UserContactEntity> userContactsToUpdate)
        {
            var now = m_dateTimeProvider.UtcNow;

            foreach (var userContactEntity in userContactsToUpdate)
            {
                Update(userContactEntity, now);
            }
        }

        public IList<UserContactEntity> GetUserContactsWithLoaGreaterThan(string contactValue, ContactTypeEnum contactTypeEnum, LevelOfAssuranceEnum levelOfAssurance)
        {
            return m_userContactRepository.GetActualVersionOfUserContactWithLoaGreaterThan(contactValue, contactTypeEnum, levelOfAssurance);
        }

        public void UpdateUserContact(UserContactEntity userContact)
        {
            var now = m_dateTimeProvider.UtcNow;

            Update(userContact, now);
        }

        public void CreateUserContacts(IEnumerable<UserContactEntity> userContactsToUpdate)
        {
            var now = m_dateTimeProvider.UtcNow;

            foreach (var userContactEntity in userContactsToUpdate)
            {
                userContactEntity.ActiveFrom = now;
                m_userContactRepository.Create(userContactEntity);
            }
        }

        private void Update(UserContactEntity userContact, DateTime now)
        {
            m_userContactRepository.EvictUserContact(userContact);

            var contact = m_userContactRepository.FindById<UserContactEntity>(userContact.Id);

            //Create new version only if some property has changed
            if (IsNewVersionNeeded(userContact, contact))
            {
                contact.ActiveTo = now;
                m_userContactRepository.Update(contact);

                var newVersion = CreateNewVersion(userContact, now);
                m_userContactRepository.Create(newVersion);
            }
        }

        private bool IsNewVersionNeeded(UserContactEntity userContact, UserContactEntity contact)
        {
            return !m_userContactEqualityComparer.Equals(userContact, contact);
        }

        private UserContactEntity CreateNewVersion(UserContactEntity userContactEntity, DateTime now)
        {
            return new UserContactEntity
            {
                ActiveFrom = now,
                ConfirmCode = userContactEntity.ConfirmCode,
                ConfirmCodeChangeTime = userContactEntity.ConfirmCodeChangeTime,
                DataSource = userContactEntity.DataSource,
                LevelOfAssurance = userContactEntity.LevelOfAssurance,
                Type = userContactEntity.Type,
                User = userContactEntity.User,
                Value = userContactEntity.Value,
            };
        }
    }
}