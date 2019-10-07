using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.DataTypes;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Core.DataEntities.Shared.Query;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;
using Ridics.Core.Shared.Types;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class UserRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<ExternalLoginEntity>> m_externalLoginOrdering;

        public UserRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_externalLoginOrdering = new List<QueryOrderBy<ExternalLoginEntity>>
            {
                new QueryOrderBy<ExternalLoginEntity> {Expression = x => x.ProviderKey}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<UserEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.PersistedGrants)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.Roles)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().Permissions)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissionTypeActions)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissionTypeActions.First().ResourcePermissionType)
                //.Fetch(SelectMode.Fetch, x => x.Roles.First().Permissions.First().Roles)
                //.Fetch(SelectMode.Fetch, x => x.Roles.First().Permissions.First().Roles.First().Permissions)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions)
                //.Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions.First().Roles)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions.First().ResourceTypeAction)
                .Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions.First().ResourceTypeAction.ResourcePermissionType)
                //.Fetch(SelectMode.Fetch, x => x.Roles.First().ResourcePermissions.First().Roles.First().ResourcePermissions)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().ResourcePermissionType)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().ResourcePermissions)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().Users)
                //.Fetch(SelectMode.Fetch, x => x.ResourcePermissionTypeActions.First().Roles)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ExternalLogins)
                .Fetch(SelectMode.Fetch, x => x.ExternalLogins.First().Provider)
                .Fetch(SelectMode.Fetch, x => x.ExternalLogins.First().Provider.Logo)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.TwoFactorLogins)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.UserClaims)
                .Fetch(SelectMode.Fetch, x => x.UserClaims.First().ClaimType)
                .Fetch(SelectMode.Fetch, x => x.UserClaims.First().ClaimType.Type)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ExternalIdentities)
                .Fetch(SelectMode.Fetch, x => x.ExternalIdentities.First().ExternalIdentityType)
                .Future<UserEntity>();

            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions.First().ResourceTypeAction)
                .Fetch(SelectMode.Fetch, x => x.ResourcePermissions.First().ResourceTypeAction.ResourcePermissionType)
                .Future<UserEntity>();
        }

        private void FetchExternalLoginCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<ExternalLoginEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.User)
                .Fetch(SelectMode.Fetch, x => x.Provider)
                .Fetch(SelectMode.Fetch, x => x.Provider.Logo)
                .Future<ExternalLoginEntity>();
        }

        public UserEntity GetUserByUsername(string username)
        {
            var criterion = Restrictions.Where<UserEntity>(x => x.Username == username);

            try
            {
                return GetUser(criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by username operation failed", ex);
            }
        }


        public UserEntity GetUserById(int id)
        {
            var criterion = Restrictions.Where<UserEntity>(x => x.Id == id);

            try
            {
                return GetUser(criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by id operation failed", ex);
            }
        }

        public IList<UserEntity> FindUsers(int start, int count, UserType type, string searchByName = null)
        {
            UserEntity userAlias = null;
            try
            {
                var session = GetSession();

                var query = session.QueryOver(() => userAlias);

                query = AddRestrictionOnUserTypeToQuery(query, type);
                query = AddSearchByNameToQuery(query, searchByName);
                query = AddOrderingByLastNameAndFirstNameToQuery(query, userAlias);

                var result = query
                    .Skip(start)
                    .Take(count)
                    .Future<UserEntity>();

                FetchCollections(session);

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get users list operation failed", ex);
            }
        }

        public int GetUsersCount(UserType type, string searchByName)
        {
            try
            {
                var session = GetSession();

                var query = session.QueryOver<UserEntity>();

                query = AddRestrictionOnUserTypeToQuery(query, type);
                query = AddSearchByNameToQuery(query, searchByName);
                

                var result = query
                    .RowCount();

                return result;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get users count operation failed", ex);
            }
        }

        public IList<UserEntity> FindUsersByRole(UserType type, int roleId, int start, int count, string searchByName)
        {
            UserEntity userAlias = null;
            try
            {
                var session = GetSession();

                var usersWithRoleNotFromAuthServiceWithId = QueryOver.Of<UserEntity>()
                    .JoinQueryOver<RoleEntity>(m => m.Roles)
                    .Where(a => !a.AuthenticationServiceOnly).And(a => a.Id == roleId)
                    .Select(Projections.Distinct(Projections.Property<UserEntity>(m => m.Id)));

                var query = session.QueryOver(() => userAlias);

                query = AddRestrictionOnUserTypeToQuery(query, type);
                query = AddOrderingByLastNameAndFirstNameToQuery(query, userAlias);
                query = AddSearchByNameToQuery(query, searchByName);

                var result = query
                    .WithSubquery.WhereProperty(x => x.Id)
                    .In(usersWithRoleNotFromAuthServiceWithId) //TODO replace subquery by where clause
                    .Skip(start) // TODO replace with GetValuesList(start, count, ...)
                    .Take(count)
                    .Future<UserEntity>();

                FetchCollections(session);

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get users list operation failed", ex);
            }
        }

        public int GetUsersByRoleCount(UserType type, int roleId, string searchByName)
        {
            UserEntity userAlias = null;
            try
            {
                var session = GetSession();

                var usersWithRoleNotFromAuthServiceWithId = QueryOver.Of<UserEntity>()
                    .JoinQueryOver<RoleEntity>(m => m.Roles)
                    .Where(a => !a.AuthenticationServiceOnly).And(a => a.Id == roleId)
                    .Select(Projections.Distinct(Projections.Property<UserEntity>(m => m.Id)));

                var query = session.QueryOver(() => userAlias);

                query = AddRestrictionOnUserTypeToQuery(query, type);
                query = AddSearchByNameToQuery(query, searchByName);

                var result = query
                    .WithSubquery.WhereProperty(x => x.Id)
                    .In(usersWithRoleNotFromAuthServiceWithId) //TODO replace subquery by where clause
                    .RowCount();

                return result;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get users count operation failed", ex);
            }
        }

        public UserEntity GetUserByValidContact(string contactValue, ContactTypeEnum contactType)
        {
            try
            {
                var session = GetSession();

                var result = session.QueryOver<UserEntity>()
                    .JoinQueryOver<UserContactEntity>(x => x.UserContacts)
                    .Where(x => x.Type == contactType && x.Value == contactValue && x.ActiveTo == null)
                    .FutureValue<UserEntity>();

                FetchCollections(session);

                return result.Value;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by contact operation failed", ex);
            }
        }

        public UserEntity GetUserByValidContact(string contactValue, ContactTypeEnum contactType, LevelOfAssuranceEnum minLevelOfAssurance)
        {
            LevelOfAssuranceEntity levelOfAssuranceAlias = null;
            UserContactEntity userContactAlias = null;
            try
            {
                var session = GetSession();

                var result = session.QueryOver<UserEntity>()
                    .JoinAlias(x => x.UserContacts, () => userContactAlias)
                    .JoinAlias(() => userContactAlias.LevelOfAssurance, () => levelOfAssuranceAlias)
                    .Where(x => userContactAlias.Type == contactType && userContactAlias.Value == contactValue && userContactAlias.ActiveTo == null && levelOfAssuranceAlias.Level >= (int)minLevelOfAssurance)
                    .FutureValue<UserEntity>();

                FetchCollections(session);

                return result.Value;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by contact operation failed", ex);
            }
        }

        public UserEntity GetUserByDataType(string dataType, string dataValue, DateTime now, bool fetchCollections = true)
        {
            UserDataEntity userDataAlias = null;
            UserDataTypeEntity userDataTypeAlias = null;

            try
            {
                var session = GetSession();

                var result = session.QueryOver<UserEntity>()
                    .JoinAlias(x => x.UserData, () => userDataAlias)
                    .JoinAlias(() => userDataAlias.UserDataType, () => userDataTypeAlias)
                    .Where(() => (userDataAlias.ActiveTo == null || userDataAlias.ActiveTo > now) && userDataTypeAlias.DataTypeValue == dataType && userDataAlias.Value == dataValue)
                    .FutureValue<UserEntity>();

                if (fetchCollections)
                {
                    FetchCollections(session);
                }

                return result.Value;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by data type operation failed", ex);
            }
        }

        public IList<UserEntity> FindUsersByDataType(string dataType, IEnumerable<string> dataValues, DateTime now, bool fetchCollections = true)
        {
            UserDataEntity userDataAlias = null;
            UserDataTypeEntity userDataTypeAlias = null;

            try
            {
                var session = GetSession();

                var result = session.QueryOver<UserEntity>()
                    .JoinAlias(x => x.UserData, () => userDataAlias)
                    .JoinAlias(() => userDataAlias.UserDataType, () => userDataTypeAlias)
                    .Where(() => (userDataAlias.ActiveTo == null || userDataAlias.ActiveTo > now) && userDataTypeAlias.DataTypeValue == dataType)
                    .WhereRestrictionOn(() => userDataAlias.Value).IsIn(dataValues.ToList())
                    .Future<UserEntity>();

                if (fetchCollections)
                {
                    FetchCollections(session);
                }

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user list by data type operation failed", ex);
            }
        }

        public UserEntity GetUserByExternalProvider(string loginProvider, string providerKey)
        {
            try
            {
                var session = GetSession();

                ExternalLoginProviderEntity provider = null;

                var result = session.QueryOver<UserEntity>()
                    .JoinQueryOver<ExternalLoginEntity>(x => x.ExternalLogins)
                    .JoinAlias(x => x.Provider, () => provider)
                    .Where(x => provider.Name == loginProvider && x.ProviderKey == providerKey)
                    .FutureValue<UserEntity>();

                FetchCollections(session);

                return result.Value;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by external provider operation failed", ex);
            }
        }

        public ExternalLoginEntity GetExternalLoginByUserAndLoginId(int userId, int externalLoginId)
        {
            var criterion = Restrictions.And(Restrictions.Where<ExternalLoginEntity>(x => x.User.Id == userId),
                Restrictions.Where<ExternalLoginEntity>(x => x.Id == externalLoginId));

            try
            {
                return GetSingleValue<ExternalLoginEntity>(FetchExternalLoginCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find two factor login for user operation failed", ex);
            }
        }

        public IList<ExternalLoginEntity> FindAllExternalLoginByProvider(int externalLoginProviderId)
        {
            var criterion = Restrictions.Where<ExternalLoginEntity>(x => x.Provider.Id == externalLoginProviderId);

            try
            {
                return GetValuesList<ExternalLoginEntity>(FetchExternalLoginCollections, criterion, null, m_externalLoginOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get externalLogin list operation failed", ex);
            }
        }

        public TwoFactorLoginEntity GetTwoFactorTokenForUser(int userId, string tokenProvider)
        {
            var criterion = Restrictions.And(Restrictions.Where<TwoFactorLoginEntity>(x => x.User.Id == userId),
                Restrictions.Where<TwoFactorLoginEntity>(x => x.TokenProvider == tokenProvider));

            try
            {
                return GetSingleValue<TwoFactorLoginEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find two factor login for user operation failed", ex);
            }
        }


        public UserEntity GetUserByVerificationCode(string verificationCode)
        {
            var criterion = Restrictions.Where<UserEntity>(x => x.VerificationCode == verificationCode);

            try
            {
                return GetUser(criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by verification code operation failed", ex);
            }
        }

        public void UpdateUserExternalIdentities(IList<UserExternalIdentityEntity> userIdentities)
        {
            try
            {
                SaveAll(userIdentities);
            }
            catch (HibernateException ex)
            {
                throw new SaveEntityException<UserExternalIdentityEntity>("Update user external identities operation failed", ex);
            }
        }

        public int CreateUser(UserEntity user)
        {
            try
            {
                var userId = (int) Create(user);
                return userId;
            }
            catch (DatabaseException e)
            {
                throw new SaveEntityException<UserEntity>("Can not create user entity", e);
            }
        }

        public IList<UserEntity> GetUsersById(IEnumerable<int> userIds)
        {
            var criterion = Restrictions.On<UserEntity>(x => x.Id).IsIn(userIds.ToList());

            try
            {
                return GetValuesList<UserEntity>(null, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get users list by ids operation failed", ex);
            }
        }

        public UserEntity GetUserByDataTypeWithMinLoa(string dataType, string dataValue, DateTime now, LevelOfAssuranceEnum levelOfAssurance, bool fetchCollections = true)
        {
            UserDataEntity userDataAlias = null;
            UserDataTypeEntity userDataTypeAlias = null;
            LevelOfAssuranceEntity levelOfAssuranceAlias = null;
            try
            {
                var session = GetSession();

                var result = session.QueryOver<UserEntity>()
                    .JoinAlias(x => x.UserData, () => userDataAlias)
                    .JoinAlias(() => userDataAlias.UserDataType, () => userDataTypeAlias)
                    .JoinAlias(x => userDataAlias.LevelOfAssurance, () => levelOfAssuranceAlias)
                    .Where(() => (userDataAlias.ActiveTo == null || userDataAlias.ActiveTo > now) && userDataTypeAlias.DataTypeValue == dataType && userDataAlias.Value == dataValue &&
                                 levelOfAssuranceAlias.Level >= (int)levelOfAssurance)
                    .FutureValue<UserEntity>();

                if (fetchCollections)
                {
                    FetchCollections(session);
                }

                return result.Value;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user by data type with min loa operation failed", ex);
            }
        }

        private UserEntity GetUser(ICriterion criterion = null)
        {
            return GetSingleValue<UserEntity>(FetchCollections, criterion);
        }

        private IQueryOver<UserEntity, UserEntity> AddRestrictionOnUserTypeToQuery(IQueryOver<UserEntity, UserEntity> query, UserType type)
        {
            if (type == UserType.NonAuthenticationService)
            {
                var usersWithRoleFromAuthService = QueryOver.Of<UserEntity>()
                    .JoinQueryOver<RoleEntity>(m => m.Roles)
                    .Where(a => a.AuthenticationServiceOnly)
                    .Select(Projections.Distinct(Projections.Property<UserEntity>(m => m.Id)));

                query = query.WithSubquery.WhereProperty(x => x.Id)
                    .NotIn(usersWithRoleFromAuthService);
            }

            return query;
        }

        private IQueryOver<UserEntity, UserEntity> AddSearchByNameToQuery(IQueryOver<UserEntity, UserEntity> query, string searchByName)
        {
            UserDataEntity userDataAlias = null;
            UserDataTypeEntity userDataTypeAlias = null;

            if (!string.IsNullOrEmpty(searchByName))
            {

                var usersWithNameStartingWith = QueryOver.Of<UserEntity>()
                    .JoinAlias(x => x.UserData, () => userDataAlias)
                    .JoinAlias(x => userDataAlias.UserDataType, () => userDataTypeAlias)
                    .Where(x => userDataAlias.Value.IsInsensitiveLike(searchByName, MatchMode.Start) && (userDataTypeAlias.DataTypeValue == UserDataTypes.LastName || userDataTypeAlias.DataTypeValue == UserDataTypes.FirstName))
                    .Select(Projections.Distinct(Projections.Property<UserEntity>(m => m.Id)));

                query = query.WithSubquery.WhereProperty(x => x.Id)
                    .In(usersWithNameStartingWith);
            }

            return query;
        }

        private IQueryOver<UserEntity, UserEntity> AddOrderingByLastNameAndFirstNameToQuery(IQueryOver<UserEntity, UserEntity> query, UserEntity userAlias)
        {
            UserDataEntity userDataAlias = null;
            UserDataTypeEntity userDataTypeAlias = null;

            var lastNameUserData = QueryOver.Of(() => userDataAlias)
                .JoinAlias(x => x.UserDataType, () => userDataTypeAlias)
                .Where(x => userDataAlias.ActiveTo == null && userDataAlias.User.Id == userAlias.Id && userDataTypeAlias.DataTypeValue == UserDataTypes.LastName)
                .Select(x => x.Value);

            var firstNameUserData = QueryOver.Of(() => userDataAlias)
                .JoinAlias(x => x.UserDataType, () => userDataTypeAlias)
                .Where(x => userDataAlias.ActiveTo == null && userDataAlias.User.Id == userAlias.Id && userDataTypeAlias.DataTypeValue == UserDataTypes.FirstName)
                .Select(x => x.Value);

            query = query
                .OrderBy(Projections.SubQuery(lastNameUserData)).Asc
                .ThenBy(Projections.SubQuery(firstNameUserData)).Asc;

            return query;
        }
    }
}