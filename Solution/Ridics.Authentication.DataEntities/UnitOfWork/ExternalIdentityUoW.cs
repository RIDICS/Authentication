using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class ExternalIdentityUoW : UnitOfWorkBase
    {
        private readonly ExternalIdentityRepository m_externalIdentityRepository;

        public ExternalIdentityUoW(
            ISessionManager sessionManager,
            ExternalIdentityRepository externalIdentityRepository
        ) : base(sessionManager)
        {
            m_externalIdentityRepository = externalIdentityRepository;
        }
        
        [Transaction]
        public virtual IList<ExternalIdentityEntity> FindAllExternalIdentity()
        {
            var externalLoginProviders = m_externalIdentityRepository.FindAllExternalIdentity();

            return externalLoginProviders;
        }

        [Transaction]
        public virtual ExternalIdentityEntity GetExternalIdentityByName(string name)
        {
            var externalLoginProvider = m_externalIdentityRepository.GetExternalIdentityByName(name);

            if (externalLoginProvider == null)
            {
                throw new NoResultException<ExternalIdentityEntity>();
            }

            return externalLoginProvider;
        }
    }
}