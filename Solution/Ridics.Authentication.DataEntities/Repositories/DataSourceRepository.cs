using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class DataSourceRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<DataSourceEntity>> m_defaultOrdering;

        public DataSourceRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<DataSourceEntity>>
            {
                new QueryOrderBy<DataSourceEntity> {Expression = x => x.DataSource}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<DataSourceEntity>> joinAliases = null)
        {
            CreateBaseQuery<DataSourceEntity>(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.DataSource)
                .Fetch(SelectMode.Fetch, x => x.ExternalLoginProvider)
                .Fetch(SelectMode.Fetch, x => x.ExternalLoginProvider.Logo)
                .Future<DataSourceEntity>();
        }

        public IList<DataSourceEntity> FindAllDataSources()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get level of assurance list operation failed", ex);
            }
        }

        public DataSourceEntity GetDataSourceById(int id)
        {
            var criterion = Restrictions.Where<DataSourceEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<DataSourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get data source by id operation failed", ex);
            }
        }

        public DataSourceEntity GetDataSourceByDataSource(DataSourceEnum dataSourceEnum)
        {
            var criterion = Restrictions.Where<DataSourceEntity>(x => x.DataSource == dataSourceEnum);

            try
            {
                return GetSingleValue<DataSourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get data source by name operation failed", ex);
            }
        }

        public DataSourceEntity GetDataSourceByDataSource(DataSourceEnum dataSourceEnum, int externalLoginProviderId)
        {
            var criterion = Restrictions.Where<DataSourceEntity>(x =>
                x.DataSource == dataSourceEnum
                && x.ExternalLoginProvider.Id == externalLoginProviderId
            );

            try
            {
                return GetSingleValue<DataSourceEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get data source by name operation failed", ex);
            }
        }

        public IList<DataSourceEntity> FindDataSourceByDataSource(DataSourceEnum dataSourceEnum)
        {
            var criterion = Restrictions.Where<DataSourceEntity>(x => x.DataSource == dataSourceEnum);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find data source by name operation failed", ex);
            }
        }
    }
}