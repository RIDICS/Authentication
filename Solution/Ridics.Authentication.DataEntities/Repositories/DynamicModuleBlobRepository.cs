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
    public class DynamicModuleBlobRepository : RepositoryBase
    {
        public DynamicModuleBlobRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        private void FetchCollections(ISession session, ICriterion criterion = null,
            IList<QueryJoinAlias<DynamicModuleBlobEntity>> joinAliases = null)
        {
            CreateBaseQuery(session, criterion, joinAliases)
                .Fetch(SelectMode.Fetch, x => x.DynamicModule)
                .Fetch(SelectMode.Fetch, x => x.File)
                .Future<DynamicModuleBlobEntity>();
        }

        public DynamicModuleBlobEntity GetById(int id)
        {
            var criterion = Restrictions.Where<DynamicModuleBlobEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<DynamicModuleBlobEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModuleBlob by id failed", ex);
            }
        }

        public DynamicModuleBlobEntity GetByModuleIdAndType(int moduleId, DynamicModuleBlobEnum dynamicModuleBlobEnum)
        {
            var criterion = Restrictions.Where<DynamicModuleBlobEntity>(
                x => x.DynamicModule.Id == moduleId && x.Type == dynamicModuleBlobEnum
            );

            try
            {
                return GetSingleValue<DynamicModuleBlobEntity>(FetchCollections, criterion);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get DynamicModuleBlob by id failed", ex);
            }
        }
    }
}