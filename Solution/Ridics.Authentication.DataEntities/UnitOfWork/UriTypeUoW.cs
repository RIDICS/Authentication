using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UriTypeUoW : UnitOfWorkBase
    {
        private readonly UriTypeRepository m_uriTypeRepository;

        public UriTypeUoW(ISessionManager sessionManager, UriTypeRepository uriTypeRepository) : base(sessionManager)
        {
            m_uriTypeRepository = uriTypeRepository;
        }

        [Transaction]
        public virtual IList<UriTypeEntity> GetAllUriTypes()
        {
            var uriTypes = m_uriTypeRepository.GetAllUriTypes();

            return uriTypes;
        }
    }
}