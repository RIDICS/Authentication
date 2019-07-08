using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class ClaimUoW : UnitOfWorkBase
    {
        private readonly ClaimRepository m_claimRepository;
        private readonly UserRepository m_userRepository;
        private readonly ClaimTypeRepository m_claimTypeRepository;

        public ClaimUoW(ISessionManager sessionManager, ClaimRepository claimRepository, UserRepository userRepository,
            ClaimTypeRepository claimTypeRepository) : base(sessionManager)
        {
            m_claimRepository = claimRepository;
            m_userRepository = userRepository;
            m_claimTypeRepository = claimTypeRepository;
        }

        [Transaction]
        public virtual IList<ClaimEntity> GetUserClaimsByUserId(int userId)
        {
            var claims = m_claimRepository.GetClaimsForUser(userId);

            return claims;
        }

        [Transaction]
        public virtual int AddClaimToUser(int userId, ClaimEntity newClaim, int claimTypeId)
        {
            var user = m_userRepository.FindById<UserEntity>(userId);
            newClaim.User = user ?? throw new NoResultException<UserEntity>();

            var claimTypes = m_claimTypeRepository.GetAllClaimTypes();
            var claimType = claimTypes.FirstOrDefault(claim => claim.Id == claimTypeId);
            
            newClaim.ClaimType = claimType ?? throw new NoResultException<ClaimTypeEntity>();

            var result = (int) m_claimRepository.Create(newClaim);

            return result;
        }

        [Transaction]
        public virtual void RemoveClaimFromUser(int userId, int claimTypeId)
        {
            var claims = m_claimRepository.GetClaimsForUser(userId); //TODO select claim directly, not collection

            var claimToRemove = claims.FirstOrDefault(x => x.ClaimType.Id == claimTypeId);

            if (claimToRemove == null)
            {
                throw new NoResultException<ClaimEntity>();
            }

            m_claimRepository.Delete(claimToRemove);
        }
    }
}