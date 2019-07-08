using System;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Proxies;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UserContactUoW : UnitOfWorkBase
    {
        private readonly UserContactVersioningProxy m_userContactRepository;
        private readonly LevelOfAssuranceRepository m_levelOfAssuranceRepository;
        private readonly DataSourceRepository m_dataSourceRepository;

        public UserContactUoW(
            ISessionManager sessionManager,
            UserContactVersioningProxy userContactRepository,
            LevelOfAssuranceRepository levelOfAssuranceRepository,
            DataSourceRepository dataSourceRepository) :
            base(sessionManager)
        {
            m_userContactRepository = userContactRepository;
            m_levelOfAssuranceRepository = levelOfAssuranceRepository;
            m_dataSourceRepository = dataSourceRepository;
        }

        [Transaction]
        public virtual void SaveConfirmCode(int userId, string code, ContactTypeEnum contactTypeEnum)
        {
            var userContact = m_userContactRepository.GetUserContact(userId, contactTypeEnum);

            if (userContact == null)
            {
                throw new NoResultException<UserContactEntity>();
            }

            userContact.ConfirmCode = code;
            userContact.ConfirmCodeChangeTime = DateTime.UtcNow;

            m_userContactRepository.UpdateUserContact(userContact);
        }

        [Transaction]
        public virtual bool ConfirmCode(int userId, string code, ContactTypeEnum contactTypeEnum)
        {
            var userContact = m_userContactRepository.GetUserContact(userId, contactTypeEnum);
            return ConfirmCode(userContact, code);
        }

        private bool ConfirmCode(UserContactEntity userContact, string code)
        {
            if (userContact == null)
            {
                throw new NoResultException<UserContactEntity>();
            }

            if (userContact.ConfirmCode == code)
            {
                userContact.ConfirmCode = null;
                userContact.ConfirmCodeChangeTime = DateTime.UtcNow;

                // if we can trust confirmation method, data is trustworthy
                HydrateContactLevelOfAssurance(userContact, LevelsOfAssurance.ContactLoaAfterConfirmation);

                m_userContactRepository.UpdateUserContact(userContact);

                return true;
            }

            return false;
        }

        [Transaction]
        public virtual void ChangeContact(int userId, UserContactEntity newContact)
        {
            var userContact = m_userContactRepository.GetUserContact(userId, newContact.Type);

            if (userContact == null)
            {
                throw new NoResultException<UserContactEntity>();
            }

            UpdateContact(userContact, newContact);
        }

        private void UpdateContact(UserContactEntity userContact, UserContactEntity newContact)
        {
            if (!userContact.Value.Equals(newContact.Value))
            {
                HydrateContactLevelOfAssurance(userContact, LevelsOfAssurance.ContactLoaAfterChange);
                HydrateContactDataSource(userContact);
            }

            userContact.Value = newContact.Value;
            userContact.ConfirmCode = newContact.ConfirmCode;
            userContact.ConfirmCodeChangeTime = newContact.ConfirmCodeChangeTime;

            m_userContactRepository.UpdateUserContact(userContact);
        }


        [Transaction]
        public virtual string GetUserConfirmCode(int userId, ContactTypeEnum contactType)
        {
            return m_userContactRepository.GetUserContact(userId, contactType).ConfirmCode;
        }

        [Transaction]
        public virtual UserContactEntity GetUserContact(int userId, ContactTypeEnum contactType)
        {
            var contact = m_userContactRepository.GetUserContact(userId, contactType);

            if (contact == null)
            {
                throw new NoResultException<UserContactEntity>();
            }

            return contact;
        }

        private void HydrateContactLevelOfAssurance(
            UserContactEntity userContactEntity,
            LevelOfAssuranceEnum levelOfAssuranceEnum
        )
        {
            userContactEntity.LevelOfAssurance = m_levelOfAssuranceRepository.GetLevelOfAssuranceByName(
                levelOfAssuranceEnum
            );
        }

        private void HydrateContactDataSource(UserContactEntity userContactEntity)
        {
            userContactEntity.DataSource = m_dataSourceRepository.GetDataSourceByDataSource(
                DataSourceEnum.User
            );
        }
    }
}
