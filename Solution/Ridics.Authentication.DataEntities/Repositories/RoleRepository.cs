using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class RoleRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<RoleEntity>> m_defaultOrdering;

        public RoleRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<RoleEntity>>
            {
                new QueryOrderBy<RoleEntity> {Expression = x => x.Name}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<RoleEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.User)
                .Fetch(SelectMode.Fetch, x => x.Permissions)
                //.Fetch(SelectMode.Fetch, x => x.Permissions.First().Roles)
                .Future<RoleEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissions.First().Roles)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissions.First().Users)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions.First().ResourceTypeAction)
                .Future<RoleEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().ResourcePermissionType)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().Roles)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().Users)
                .Future<RoleEntity>();
        }

        private Action<ISession, ICriterion, IList<QueryJoinAlias<RoleEntity>>> GetFetchMethod(bool isFetchEnabled)
        {
            Action<ISession, ICriterion, IList<QueryJoinAlias<RoleEntity>>> fetchMethod = FetchCollections;
            if (!isFetchEnabled)
            {
                fetchMethod = null;
            }

            return fetchMethod;
        }

        private AbstractCriterion CreateSearchCriteria(string searchByName)
        {
            var criteria = string.IsNullOrEmpty(searchByName)
                ? null
                : Restrictions.On<RoleEntity>(x => x.Name).IsInsensitiveLike(searchByName, MatchMode.Anywhere);

            return criteria;
        }

        public IList<RoleEntity> GetAllRoles(bool fetch)
        {
            try
            {
                return GetValuesList(GetFetchMethod(fetch), null, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all role list operation failed", ex);
            }
        }

        public IList<RoleEntity> GetAllNonAuthenticationServiceRoles(bool fetch)
        {
            var criteria = Restrictions.WhereNot<RoleEntity>(x => x.AuthenticationServiceOnly);

            try
            {
                return GetValuesList<RoleEntity>(GetFetchMethod(fetch), criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get all non authentication service role list operation failed", ex);
            }
        }

        public IList<RoleEntity> GetRolesById(IEnumerable<int> ids, bool fetch)
        {
            var criterion = Restrictions.On<RoleEntity>(x => x.Id).IsIn(ids.ToList());

            try
            {
                return GetValuesList<RoleEntity>(GetFetchMethod(fetch), criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get roles list by ids operation failed", ex);
            }
        }

        public IList<RoleEntity> GetRolesByUser(int userId)
        {
            try
            {
                var session = GetSession();

                var result = session.QueryOver<RoleEntity>()
                    .OrderBy(x => x.Name).Asc
                    .JoinQueryOver<UserEntity>(x => x.User)
                    .Where(u => u.Id == userId)
                    .TransformUsing(new DistinctRootEntityResultTransformer())
                    .Future<RoleEntity>();

                FetchCollections(session);

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get roles by user operation failed", ex);
            }
        }

        public RoleEntity FindRoleById(int id, bool fetch)
        {
            var criterion = Restrictions.Where<RoleEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<RoleEntity>(GetFetchMethod(fetch), criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find role by id operation failed", ex);
            }
        }

        public RoleEntity FindRoleByName(string name, bool fetch)
        {
            var criterion = Restrictions.Where<RoleEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<RoleEntity>(GetFetchMethod(fetch), criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find role by name operation failed", ex);
            }
        }

        public IList<RoleEntity> GetRoles(int start, int count, string searchByName = null, bool fetch = false)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesList(start, count, GetFetchMethod(fetch), criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get roles operation failed", ex);
            }
        }


        public int GetRolesCount(string searchByName = null)
        {
            var criteria = CreateSearchCriteria(searchByName);

            try
            {
                return GetValuesCount<RoleEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get roles count operation failed", ex);
            }
        }

        public IList<RoleEntity> GetNonAuthenticationServiceRoles(int start, int count, string searchByName = null, bool fetch = false)
        {
            var criteria = Restrictions.WhereNot<RoleEntity>(x => x.AuthenticationServiceOnly);

            if (!string.IsNullOrEmpty(searchByName))
            {
                criteria = new Conjunction()
                    .Add(criteria)
                    .Add(CreateSearchCriteria(searchByName));
            }

            try
            {
                return GetValuesList(start, count, GetFetchMethod(fetch), criteria, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get non authentication service roles operation failed", ex);
            }
        }

        public int GetNonAuthenticationServiceRolesCount(string searchByName = null)
        {
            var criteria = Restrictions.WhereNot<RoleEntity>(x => x.AuthenticationServiceOnly);

            if (!string.IsNullOrEmpty(searchByName))
            {
                criteria = new Conjunction()
                    .Add(criteria)
                    .Add(CreateSearchCriteria(searchByName));
            }

            try
            {
                return GetValuesCount<RoleEntity>(criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get roles count operation failed", ex);
            }
        }

        public RoleEntity GetRoleByName(string roleName)
        {
            var criterion = Restrictions.Where<RoleEntity>(x => x.Name == roleName);

            try
            {
                return GetSingleValue<RoleEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get role by name operation failed", ex);
            }
        }
    }
}