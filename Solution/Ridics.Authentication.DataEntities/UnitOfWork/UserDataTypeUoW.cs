using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UserDataTypeUoW : UnitOfWorkBase
    {
        private readonly UserDataTypeRepository m_userDataTypeRepository;

        public UserDataTypeUoW(ISessionManager sessionManager, UserDataTypeRepository userDataTypeRepository) : base(sessionManager)
        {
            m_userDataTypeRepository = userDataTypeRepository;
        }

        [Transaction]
        public virtual IList<UserDataTypeEntity> GetAllUserDataTypes()
        {
            var userDataTypes = m_userDataTypeRepository.GetAllUserDataTypes();

            return userDataTypes;
        }

        [Transaction]
        public virtual int CreateUserDataType(UserDataTypeEntity userDataType)
        {
            var result = (int)m_userDataTypeRepository.Create(userDataType);

            return result;
        }

        [Transaction]
        public virtual UserDataTypeEntity FindUserDataTypeById(int id)
        {
            var userData = m_userDataTypeRepository.FindById<UserDataTypeEntity>(id);

            if (userData == null)
            {
                throw new NoResultException<UserDataEntity>();
            }

            return userData;
        }

        [Transaction]
        public virtual UserDataTypeEntity FindUserDataTypeByValue(string value)
        {
            var userDataType = m_userDataTypeRepository.GetUserDataTypeByValue(value);

            if (userDataType == null)
            {
                throw new NoResultException<UserDataEntity>();
            }

            return userDataType;
        }
    }
}