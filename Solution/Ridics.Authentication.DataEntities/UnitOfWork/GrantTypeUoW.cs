using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class GrantTypeUoW : UnitOfWorkBase
    {
        private readonly GrantTypeRepository m_grantTypeRepository;

        public GrantTypeUoW(ISessionManager sessionManager, GrantTypeRepository grantTypeRepository) : base(sessionManager)
        {
            m_grantTypeRepository = grantTypeRepository;
        }

        [Transaction]
        public virtual IList<GrantTypeEntity> GetAllGrantTypes()
        {
            var grantTypes = m_grantTypeRepository.GetAllGrantTypes();

            return grantTypes;
        }
    }
}