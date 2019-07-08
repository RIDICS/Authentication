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
    public class UserContactRepository : RepositoryBase
    {
        private readonly List<QueryOrderBy<UserContactEntity>> m_defaultOrdering;

        public UserContactRepository(ISessionManager sessionManager) : base(sessionManager)
        {
            m_defaultOrdering = new List<QueryOrderBy<UserContactEntity>>
            {
                new QueryOrderBy<UserContactEntity> {Expression = x => x.Value}
            };
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<UserContactEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.User)
                .Fetch(SelectMode.Fetch, x => x.LevelOfAssurance)
                .Fetch(SelectMode.Fetch, x => x.DataSource)
                .Future();
        }

        public UserContactEntity GetActualVersionOfUserContact(int userId, ContactTypeEnum contactTypeEnum)
        {
            var criterion = Restrictions.Where<UserContactEntity>(x => x.User.Id == userId && x.Type == contactTypeEnum && x.ActiveTo == null);

            try
            {
                return GetSingleValue<UserContactEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user contact operation failed", ex);
            }
        }

        public IList<UserContactEntity> GetActualVersionOfUserContactWithLoaGreaterThan(string contactValue, ContactTypeEnum contactTypeEnum, LevelOfAssuranceEnum levelOfAssurance)
        {
            LevelOfAssuranceEntity levelOfAssuranceAlias = null;

            try
            {
                var session = GetSession();
                var result = session.QueryOver<UserContactEntity>()
                    .JoinAlias(x => x.LevelOfAssurance, () => levelOfAssuranceAlias)
                    .Where(x => x.Type == contactTypeEnum && x.Value == contactValue && x.ActiveTo == null &&
                                levelOfAssuranceAlias.Level > (int)levelOfAssurance)
                    .Future<UserContactEntity>();
                FetchCollections(session);

                return result.ToList();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user contact operation failed", ex);
            }
        }

        public IList<UserContactEntity> GetActualVersionsOfUserContacts(int userId)
        {
            var criterion = Restrictions.Where<UserContactEntity>(x => x.User.Id == userId && x.ActiveTo == null);

            try
            {
                return GetValuesList(FetchCollections, criterion, null, m_defaultOrdering);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Find user contacts operation failed", ex);
            }
        }

        public void EvictUserContact(UserContactEntity userContactEntity)
        {
            var session = GetSession();

            if (session.Contains(userContactEntity))
            {
                session.Evict(userContactEntity);
            }
        }
    }
}