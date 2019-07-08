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
    public class DynamicModuleRepository : RepositoryBase
    {
        public DynamicModuleRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<DynamicModuleEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.DynamicModuleBlobs)
                .Fetch(SelectMode.Fetch, x => x.DynamicModuleBlobs.First().File)
                .Future<DynamicModuleEntity>();
        }

        public IList<DynamicModuleEntity> FindAllDynamicModule()
        {
            try
            {
                return GetValuesList<DynamicModuleEntity>(FetchCollections);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModule list failed", ex);
            }
        }

        public IList<DynamicModuleEntity> FindAllDynamicModule(int start, int count)
        {
            try
            {
                return GetValuesList<DynamicModuleEntity>(start, count, FetchCollections);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModule list failed", ex);
            }
        }

        public DynamicModuleEntity GetByName(string name)
        {
            var criterion = Restrictions.Where<DynamicModuleEntity>(x => x.Name == name);

            try
            {
                return GetSingleValue<DynamicModuleEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModule by name failed", ex);
            }
        }

        public DynamicModuleEntity GetById(int id)
        {
            var criterion = Restrictions.Where<DynamicModuleEntity>(x => x.Id == id);


            try
            {
                return GetSingleValue<DynamicModuleEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModule by id failed", ex);
            }
        }

        public int GetDynamicModuleCount()
        {
            try
            {
                return GetValuesCount<DynamicModuleEntity>();
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModule count operation failed", ex);
            }
        }
    }
}