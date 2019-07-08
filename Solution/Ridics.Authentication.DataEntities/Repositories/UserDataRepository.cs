using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserDataRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<UserDataEntity>> m_defaultOrdering;

        public UserDataRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<UserDataEntity>>
            {
                new QueryOrderBy<UserDataEntity> {Expression = x => x.Value}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<UserDataEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.User)
                .Fetch(SelectMode.Fetch, x => x.VerifiedBy)
                .Fetch(SelectMode.Fetch, x => x.LevelOfAssurance)
                .Fetch(SelectMode.Fetch, x => x.DataSource)
                .Fetch(SelectMode.Fetch, x => x.UserDataType)
                .Future();
        }

        public IList<UserDataEntity> FindCurrentVersionOfUserData(int userId, DateTime now)
        {
            var criterion = Restrictions.Where<UserDataEntity>(x => x.User.Id == userId && (x.ActiveTo == null || x.ActiveTo > now));

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user data by user id operation failed", ex);
            }
        }

        public IList<UserDataEntity> FindCurrentVersionOfUserDataWithLoaGreaterThan(string value, string dataTypeValue,
            LevelOfAssuranceEnum levelOfAssurance, DateTime now)
        {
            LevelOfAssuranceEntity levelOfAssuranceAlias = null;
            try
            {
                var session = GetSession();

                var query = session.QueryOver<UserDataEntity>()
                    .JoinAlias(x => x.LevelOfAssurance, () => levelOfAssuranceAlias)
                    .Where(x => x.Value == value && (x.ActiveTo == null || x.ActiveTo > now) &&
                                levelOfAssuranceAlias.Level > (int) levelOfAssurance);

                var result = query.JoinQueryOver(x => x.UserDataType)
                    .Where(x => x.DataTypeValue == dataTypeValue)
                    .Future<UserDataEntity>();

                FetchCollections(session);

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user data by value and type operation failed", ex);
            }
        }

        public void EvictUserData(UserDataEntity userData)
        {
            var session = GetSession();

            if (session.Contains(userData))
            {
                session.Evict(userData);
            }
        }

        public void EvictUserData(IList<UserDataEntity> userData)
        {
            if (userData == null)
            {
                return;
            }

            foreach (var userDataEntity in userData)
            {
                EvictUserData(userDataEntity);
            }
        }
    }
}