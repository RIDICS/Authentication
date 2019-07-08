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
    public class ResourcePermissionTypeActionRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ResourcePermissionTypeActionEntity>> m_defaultOrdering;

        public ResourcePermissionTypeActionRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ResourcePermissionTypeActionEntity>>
            {
                new QueryOrderBy<ResourcePermissionTypeActionEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ResourcePermissionTypeActionEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionType)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions)
                .Fetch(SelectMode.Fetch, x => x.Roles)
                .Fetch(SelectMode.Fetch, x => x.Users)
                .Future<ResourcePermissionTypeActionEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<ResourcePermissionTypeActionEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<ResourcePermissionTypeActionEntity> GetPermissionTypeActions(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission type actions operation failed", ex);
            }
        }

        public int GetPermissionTypeActionsCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<ResourcePermissionTypeActionEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission type actions count operation failed", ex);
            }
        }

        public ResourcePermissionTypeActionEntity FindPermissionTypeActionById(int id)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeActionEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ResourcePermissionTypeActionEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource permission type action by id operation failed", ex);
            }
        }

        public ResourcePermissionTypeActionEntity FindPermissionTypeActionByName(string name)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeActionEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ResourcePermissionTypeActionEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource permission type action by name operation failed", ex);
            }
        }

        public IList<ResourcePermissionTypeActionEntity> GetAllPermissionTypeActions()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all resource permission type actions list operation failed", ex);
            }
        }

        public IList<ResourcePermissionTypeActionEntity> GetPermissionTypeActionsById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ResourcePermissionTypeActionEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission type actions list by ids operation failed", ex);
            }
        }

        public IList<ResourcePermissionTypeActionEntity> GetActionsForResourcePermissionTypeById(int id)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeActionEntity>(x => x.ResourcePermissionType.Id == id);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource permission type actions list for resource permission type operation failed", ex);
            }
        }
    }
}