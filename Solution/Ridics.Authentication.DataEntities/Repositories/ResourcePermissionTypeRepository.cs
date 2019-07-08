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
    public class ResourcePermissionTypeRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ResourcePermissionTypeEntity>> m_defaultOrdering;

        public ResourcePermissionTypeRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ResourcePermissionTypeEntity>>
            {
                new QueryOrderBy<ResourcePermissionTypeEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ResourcePermissionTypeEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions)
                .Future<ResourcePermissionTypeEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ResourcePermissionTypeEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ResourcePermissionTypeEntity> GetPermissionTypes(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission types operation failed", ex);
            }
        }

        public int GetPermissionTypesCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ResourcePermissionTypeEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission type count operation failed", ex);
            }
        }

        public ResourcePermissionTypeEntity FindPermissionTypeById(int id)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ResourcePermissionTypeEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource permission type by id operation failed", ex);
            }
        }

        public ResourcePermissionTypeEntity FindPermissionTypeByName(string name)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ResourcePermissionTypeEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource permission type by name operation failed", ex);
            }
        }

        public IList<ResourcePermissionTypeEntity> GetAllPermissionTypes()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all resource permission types list operation failed", ex);
            }
        }

        public IList<ResourcePermissionTypeEntity> GetPermissionTypesById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ResourcePermissionTypeEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission types list by ids operation failed", ex);
            }
        }
    }
}