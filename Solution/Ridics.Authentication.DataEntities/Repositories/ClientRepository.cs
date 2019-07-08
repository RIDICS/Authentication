using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class ClientRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ClientEntity>> m_defaultOrdering;

        public ClientRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ClientEntity>>
            {
                new QueryOrderBy<ClientEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ClientEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.AllowedGrantTypes)
                .Future<ClientEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.AllowedIdentityResources)
                .Fetch(SelectMode.Fetch, x => x.AllowedIdentityResources.First().ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.AllowedIdentityResources.First().ClaimTypes.First().Type)
                .Future<ClientEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.PersistedGrants)
                .Future<ClientEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Secrets)
                .Future<ClientEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.UriList)
                .Fetch(SelectMode.Fetch, x => x.UriList.First().UriTypes)
                .Future<ClientEntity>();

            CreateBaseQuery<ClientEntity>(session, criterion)
                .Fetch(SelectMode.Fetch, x => x.AllowedScopes)
                .Fetch(SelectMode.Fetch, x => x.AllowedScopes.First().ClaimTypes)
                .Fetch(SelectMode.Fetch, x => x.AllowedScopes.First().ClaimTypes.First().Type)
                .Future<ClientEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ClientEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ClientEntity> GetClients(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get client list operation failed", ex);
            }
        }

        public int GetClientsCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ClientEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get client list operation failed", ex);
            }
        }

        public ClientEntity FindClientByName(string name)
        {
            var criterion = Restrictions.Where<ClientEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ClientEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find client by name operation failed", ex);
            }
        }

        public ClientEntity FindClientById(int id)
        {
            var criterion = Restrictions.Where<ClientEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ClientEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find client by name operation failed", ex);
            }
        }
    }
}