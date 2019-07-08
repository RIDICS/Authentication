using System;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Core.DataEntities.Shared.Repositories;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Repositories
{
    public class FileResourceRepository : RepositoryBase
    {
        public FileResourceRepository(ISessionManager sessionManager) : base(sessionManager)
        {
        }

        public int CreateExternal(string fileExtension)
        {
            var fileResourceEntity = new FileResourceEntity
            {
                Guid = Guid.NewGuid(),
                Type = FileResourceEnum.External,
                FileExtension = fileExtension,
            };

            var id = (int) Create(fileResourceEntity);

            return id;
        }

        public FileResourceEntity GetFileResource(int id)
        {
            var criteria = Restrictions.Where<FileResourceEntity>(x => x.Id == id);

            try
            {
                return GetSingleValue<FileResourceEntity>(null, criteria);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException("Get fileResource by id operation failed", ex);
            }
        }
    }
}
