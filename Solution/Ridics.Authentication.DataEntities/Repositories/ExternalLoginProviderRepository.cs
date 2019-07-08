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
    public class ExternalLoginProviderRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ExternalLoginProviderEntity>> m_defaultOrdering;

        public ExternalLoginProviderRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ExternalLoginProviderEntity>>
            {
                new QueryOrderBy<ExternalLoginProviderEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ExternalLoginProviderEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Logo)
                .Fetch(SelectMode.Fetch, x => x.DynamicModule)
                .Future<ExternalLoginProviderEntity>();
        }

        public IList<ExternalLoginProviderEntity> FindAllExternalLoginProviders()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find all externalLoginProvider operation failed", ex);
            }
        }

        public IList<ExternalLoginProviderEntity> FindExternalLoginProviders(int start, int count)
        {
            try
            {
                return GetValuesList<ExternalLoginProviderEntity>(start, count, FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find subset of externalLoginProvider operation failed", ex);
            }
        }

        public int GetExternalLoginProvidersCount()
        {
            try
            {
                return GetValuesCount<ExternalLoginProviderEntity>();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get count externalLoginProvider operation failed", ex);
            }
        }

        public IList<ExternalLoginProviderEntity> FindManageableExternalLoginProviders()
        {
            var criterion = Restrictions.Where<ExternalLoginProviderEntity>(x => x.DisableManagingByUser == false);

            try
            {
                return GetValuesList<ExternalLoginProviderEntity>(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find manageable externalLoginProvider operation failed", ex);
            }
        }

        public ExternalLoginProviderEntity GetExternalLoginProviderById(int id)
        {
            var criterion = Restrictions.Where<ExternalLoginProviderEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ExternalLoginProviderEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get externalLoginProvider by id operation failed", ex);
            }
        }

        public ExternalLoginProviderEntity GetExternalLoginProviderByName(string name)
        {
            var criterion = Restrictions.Where<ExternalLoginProviderEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ExternalLoginProviderEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get externalLoginProvider by name operation failed", ex);
            }
        }

        public ExternalLoginProviderEntity GetExternalLoginProviderByDynamicModuleId(int dynamicModuleId)
        {
            var criterion = Restrictions.Where<ExternalLoginProviderEntity>(x => x.DynamicModule.Id == dynamicModuleId);

            try
            {
                return GetSingleValue<ExternalLoginProviderEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get externalLoginProvider by dynamicModule operation failed", ex);
            }
        }
    }
}