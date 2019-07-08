using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class FileResourceUoW : UnitOfWorkBase
    {
        private readonly FileResourceRepository m_fileResourceRepository;

        public FileResourceUoW(
            ISessionManager sessionManager,
            FileResourceRepository fileResourceRepository
        ) : base(sessionManager)
        {
            m_fileResourceRepository = fileResourceRepository;
        }

        [Transaction]
        public virtual FileResourceEntity GetFileResource(int id)
        {
            var fileResource = m_fileResourceRepository.GetFileResource(id);

            return fileResource;
        }
    }
}