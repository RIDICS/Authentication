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
    public class PermissionRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<PermissionEntity>> m_defaultOrdering;

        public PermissionRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<PermissionEntity>>
            {
                new QueryOrderBy<PermissionEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<PermissionEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Roles)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().Permissions)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().Permissions.First().Roles)
                .Future<PermissionEntity>();
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<PermissionEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<PermissionEntity> GetPermissions(int start, int count, string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, FetchCollections, criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get policies operation failed", ex);
            }
        }

        public int GetPermissionsCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<PermissionEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get policies operation failed", ex);
            }
        }

        public PermissionEntity FindPermissionById(int id)
        {
            var criterion = Restrictions.Where<PermissionEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<PermissionEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find policy by id operation failed", ex);
            }
        }

        public PermissionEntity GetPermissionByUserByName(int userId, string permissionName)
        {
            RoleEntity roleAlias = null;
            UserEntity userAlias = null;
            
            try
            {
                var query = GetSession().QueryOver<PermissionEntity>()
                    .Where(x => x.Name == permissionName)
                    .JoinAlias(x => x.Roles, () => roleAlias)
                    .JoinAlias(() => roleAlias.User, () => userAlias)
                    .Where(() => userAlias.Id == userId)
                    .SingleOrDefault();

                return query;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find permission by name and user id operation failed", ex);
            }
        }

        public IList<PermissionEntity> GetAllPermissions()
        {
            try
            {
                return GetValuesList(FetchCollections, null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all policy list operation failed", ex);
            }
        }

        public IList<PermissionEntity> GetPermissionsById(IEnumerable<int> ids)
        {
            var criterion = Restrictions.On<PermissionEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get policy list by ids operation failed", ex);
            }
        }
    }
}