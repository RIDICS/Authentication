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
    public class ResourcePermissionRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ResourcePermissionEntity>> m_defaultOrdering;

        public ResourcePermissionRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<ResourcePermissionEntity>>
            {
                new QueryOrderBy<ResourcePermissionEntity> {Expression = x => x.ResourceId}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ResourcePermissionEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourceTypeAction)
                .Fetch(SelectMode.Fetch, x => x.ResourceTypeAction.ResourcePermissionType)
                .Fetch(SelectMode.Fetch, x => x.Roles)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions.First().Roles)
                .Fetch(SelectMode.Fetch, x => x.Users)
                .Fetch(SelectMode.Fetch, x => x.Users.First().ResourcePermissions)
                .Fetch(SelectMode.Fetch, x => x.Users.First().ResourcePermissions.First().Users)
                .Future<ResourcePermissionEntity>();
        }

        public IList<ResourcePermissionEntity> GetPermissions(int start, int count)
        {
            try
            {
                return GetValuesList(start, count, FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource policies operation failed", ex);
            }
        }

        public int GetPermissionsCount()
        {
            try
            {
                return GetValuesCount<ResourcePermissionEntity>();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource policies operation failed", ex);
            }
        }

        public ResourcePermissionEntity FindPermissionById(int id)
        {
            var criterion = Restrictions.Where<ResourcePermissionEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ResourcePermissionEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource policy by id operation failed", ex);
            }
        }

        public ResourcePermissionTypeEntity FindPermissionTypeById(int id)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<ResourcePermissionTypeEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource type by id operation failed", ex);
            }
        }

        public ResourcePermissionTypeEntity FindPermissionTypeByName(string name)
        {
            var criterion = Restrictions.Where<ResourcePermissionTypeEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<ResourcePermissionTypeEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find resource type by name operation failed", ex);
            }
        }

        public IList<ResourcePermissionEntity> GetAllPermissions()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all resource policy list operation failed", ex);
            }
        }

        public IList<ResourcePermissionEntity> GetPermissionsById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<ResourcePermissionEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get resource policy list by ids operation failed", ex);
            }
        }
    }
}