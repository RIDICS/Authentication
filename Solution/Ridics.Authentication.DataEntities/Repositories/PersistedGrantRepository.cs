using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class PersistedGrantRepository : RepositoryBase
    {
        public PersistedGrantRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<PersistedGrantEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.User)
                .Fetch(SelectMode.Fetch, x => x.Client)
                .Future<PersistedGrantEntity>();
        }

        public PersistedGrantEntity FindByKey(string key)
        {
            var criterion = Restrictions.Where<PersistedGrantEntity>(x => x.Key == key);

            try
            {
                return GetSingleValue<PersistedGrantEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find persisted grant by key operation failed", ex);
            }
        }

        public IList<PersistedGrantEntity> GetAllForUser(int userId)
        {
            var criterion = Restrictions.Where<PersistedGrantEntity>(x => x.User.Id == userId);

            try
            {
                return GetValuesList<PersistedGrantEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all persisted grant for user operation failed", ex);
            }
        }

        public IList<PersistedGrantEntity> GetAllForUserClient(int userId, int clientId)
        {
            var criterion = Restrictions.Where<PersistedGrantEntity>(x => x.User.Id == userId && x.Client.Id == clientId);

            try
            {
                return GetValuesList<PersistedGrantEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all persisted grant for user and client operation failed", ex);
            }
        }

        public IList<PersistedGrantEntity> GetAllForUserClientType(int userId, int clientId, string type)
        {
            var criterion =
                Restrictions.Where<PersistedGrantEntity>(x => x.User.Id == userId && x.Client.Id == clientId && x.Type == type);

            try
            {
                return GetValuesList<PersistedGrantEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all persisted grant for user, client and type operation failed", ex);
            }
        }
    }
}