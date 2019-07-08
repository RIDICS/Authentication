using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UserDataUoW : UnitOfWorkBase
    {
        private readonly UserDataRepository m_userDataRepository;

        public UserDataUoW(ISessionManager sessionManager, UserDataRepository userDataRepository) : base(sessionManager)
        {
            m_userDataRepository = userDataRepository;
        }

        [Transaction]
        public virtual UserDataEntity FindUserDataById(int userDataId)
        {
            var userData = m_userDataRepository.FindById<UserDataEntity>(userDataId);

            if (userData == null)
            {
                throw new NoResultException<UserDataEntity>();
            }

            return userData;
        }

        [Transaction]
        public virtual int CreateUserData(UserDataEntity userData)
        {
            var result = (int)m_userDataRepository.Create(userData);

            return result;
        }
    }
}