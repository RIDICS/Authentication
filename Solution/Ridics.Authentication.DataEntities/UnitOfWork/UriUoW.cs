using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UriUoW : UnitOfWorkBase
    {
        private readonly UriRepository m_uriRepository;
        private readonly ClientRepository m_clientRepository;

        public UriUoW(ISessionManager sessionManager, UriRepository uriRepository, ClientRepository clientRepository) :
            base(sessionManager)
        {
            m_uriRepository = uriRepository;
            m_clientRepository = clientRepository;
        }

        [Transaction]
        public virtual IList<UriEntity> FindUrisForClient(int clientId)
        {
            var uris = m_uriRepository.FindUrisForClient(clientId);

            return uris;
        }

        [Transaction]
        public virtual IList<UriEntity> GetAllUris()
        {
            var uris = m_uriRepository.GetAllUris();

            return uris;
        }

        [Transaction]
        public virtual void DeleteUriForClient(int uriId)
        {
            var uri = m_uriRepository.Load<UriEntity>(uriId);

            try
            {
                m_uriRepository.Delete(uri);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<UriEntity>();
            }
        }

        [Transaction]
        public virtual int CreateUriForClient(int clientId, UriEntity uriEntity)
        {
            var client = m_clientRepository.FindById<ClientEntity>(clientId);

            uriEntity.Client = client ?? throw new NoResultException<ClientEntity>();

            var result = (int) m_uriRepository.Create(uriEntity);

            return result;
        }
    }
}