using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataEntities.Comparer;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Authentication.DataEntities.Utils;
using Ridics.Core.Shared.Providers;

namespace Ridics.Authentication.DataEntities.Proxies
{
    public class UserDataVersioningProxy
    {
        private readonly UserDataRepository m_userDataRepository;
        private readonly UserDataEqualityComparer m_userDataEqualityComparer;
        private readonly IDateTimeProvider m_dateTimeProvider;
        private readonly UserDataStructureConvertor m_userDataStructureConvertor;

        public UserDataVersioningProxy(UserDataRepository userDataRepository, UserDataEqualityComparer userDataEqualityComparer,
            IDateTimeProvider dateTimeProvider, UserDataStructureConvertor userDataStructureConvertor)
        {
            m_userDataRepository = userDataRepository;
            m_userDataEqualityComparer = userDataEqualityComparer;
            m_dateTimeProvider = dateTimeProvider;
            m_userDataStructureConvertor = userDataStructureConvertor;
        }

        public void UpdateUserData(IEnumerable<UserDataEntity> userDataToUpdate)
        {
            var now = m_dateTimeProvider.UtcNow; //All new versions of user data should have same update time

            foreach (var userData in userDataToUpdate)
            {
                if (userData.ParentUserData != null)
                {
                    continue; //Continue because these user data were already updated
                }

                Update(userData, now);
            }
        }

        public IList<UserDataEntity> FindUserDataWithTreeStructure(int userId)
        {
            var userData = m_userDataRepository.FindCurrentVersionOfUserData(userId, m_dateTimeProvider.UtcNow);

            return GetTreeUserDataStructure(userData);
        }

        public IList<UserDataEntity> FindUserDataWithLoaGreaterThan(string value, string type, LevelOfAssuranceEnum levelOfAssurance)
        {
            var userData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(value, type, levelOfAssurance,
                    m_dateTimeProvider.UtcNow);

            return userData;
        }

        private IList<UserDataEntity> GetTreeUserDataStructure(IList<UserDataEntity> userData)
        {
            m_userDataRepository.EvictUserData(userData); //Evict list from transaction to prevent fetching/loading of children user data

            foreach (var userDataEntity in userData)
            {
                userDataEntity.ChildrenUserData =
                    new List<UserDataEntity>(); //Create new empty child list to each user data, otherwise lazy initialization exception can be thrown
            }

            return m_userDataStructureConvertor.GetTreeUserDataFromFlatUserDataList(userData);
        }

        public void CreateUserData(IEnumerable<UserDataEntity> userData)
        {
            var now = m_dateTimeProvider.UtcNow;

            foreach (var userDataEntity in userData)
            {
                Create(userDataEntity, now);
            }
        }

        /// <summary>
        /// Creates new type of user data
        /// </summary>
        /// <param name="userData">User data to create</param>
        /// <param name="now">Time of creation</param>
        private void Create(UserDataEntity userData, DateTime now)
        {
            userData.ActiveFrom = now;
            m_userDataRepository.Create(userData);
            if (userData.ChildrenUserData != null && userData.ChildrenUserData.Count != 0) CreateUserData(userData.ChildrenUserData);
        }

        /// <summary>
        /// Creates new version of userData when it is needed, i.e. property has changed.
        /// Does not create new version when new version of parent or child user data is created. In that case only updates the relation.
        /// </summary>
        /// <param name="updatedUserData">Updated user data</param>
        /// <param name="now">Time of update</param>
        /// <param name="parent">Parent user data</param>
        private void Update(UserDataEntity updatedUserData, DateTime now, UserDataEntity parent = null)
        {
            //Evict user data with updated properties, because nhibernate tracks each instance of loaded entity
            m_userDataRepository.EvictUserData(updatedUserData);

            var currentUserData = m_userDataRepository.FindById<UserDataEntity>(updatedUserData.Id);

            //This means that this type of user data have not been created yet, so create new userdata
            if (currentUserData == null)
            {
                Create(updatedUserData, now);
                return;
            }

            //Create new version only if some property has changed
            if (IsNewVersionNeeded(updatedUserData, currentUserData))
            {
                currentUserData = HandleNewVersion(updatedUserData, currentUserData, parent, now);
            }

            if (IsRefreshRelationWithParentNeeded(currentUserData, parent))
            {
                //This method is applicable only for updating children data
                //When new version was not created at least update parent of current version, if needed
                UpdateParentOfUserData(currentUserData, parent);
            }

            UpdateChildren(updatedUserData, currentUserData, now);
        }

        /// <summary>
        /// Sets active to property of current user data and creates new version from updated current version 
        /// </summary>
        /// <param name="updatedUserData">Current version with modified properties</param>
        /// <param name="currentUserData">Current version of user data</param>
        /// <param name="parent">Parent for new version</param>
        /// <param name="now">Time of update</param>
        /// <returns>Newly created version of user data</returns>
        private UserDataEntity HandleNewVersion(UserDataEntity updatedUserData, UserDataEntity currentUserData, UserDataEntity parent,
            DateTime now)
        {
            var newActiveTo = updatedUserData.ActiveTo;
            if (currentUserData.ActiveTo == null || currentUserData.ActiveTo > now)
            {
                currentUserData.ActiveTo = now;
                m_userDataRepository.Update(currentUserData);
            }

            if (newActiveTo <= now)
            {
                throw new InvalidOperationException("ActiveTo property of new UserData version must be null or greater than UtcNow");
            }

            var newVersion = CreateNewVersion(updatedUserData, now, newActiveTo, parent);

            var newVersionId = (int) m_userDataRepository.Create(newVersion);
            return m_userDataRepository.FindById<UserDataEntity>(newVersionId);
        }

        /// <summary>
        /// When parent of current user data version is different than parent, update it
        /// </summary>
        /// <param name="currentUserData">Current version of user data</param>
        /// <param name="parent">Possible new parent of current version</param>
        private void UpdateParentOfUserData(UserDataEntity currentUserData, UserDataEntity parent)
        {
            currentUserData.ParentUserData = parent;
            m_userDataRepository.Update(currentUserData);
        }

        /// <summary>
        /// Updates children
        /// </summary>
        /// <param name="updatedUserData">Current version of user data with modified properties</param>
        /// <param name="currentUserData">Current version of user data</param>
        /// <param name="now">Time of update</param>
        private void UpdateChildren(UserDataEntity updatedUserData, UserDataEntity currentUserData, DateTime now)
        {
            //Update children and save current user data version with updated children
            if (updatedUserData.ChildrenUserData != null && updatedUserData.ChildrenUserData.Count != 0)
            {
                foreach (var userDataEntity in updatedUserData.ChildrenUserData)
                {
                    Update(userDataEntity, now, currentUserData);
                }
            }
        }

        /// <summary>
        /// When the two user data are different, i.e. has one property of User, UserDataType, Verifier, Value, LoA, DataSource different, returns true
        /// </summary>
        /// <param name="updatedUserData">User data to compare</param>
        /// <param name="loadedUserData">User data to compare</param>
        /// <returns>True if the two provided user data are different, otherwise false</returns>
        private bool IsNewVersionNeeded(UserDataEntity updatedUserData, UserDataEntity loadedUserData)
        {
            return !m_userDataEqualityComparer.Equals(updatedUserData, loadedUserData);
        }

        /// <summary>
        /// When new version of parent was created relation is need to be refreshed
        /// </summary>
        /// <param name="currentUserData">Current version of user data</param>
        /// <param name="parent">Current version of parent user data</param>
        /// <returns>Returns true when relation between currentUserData and parentUserData needs to be refreshed</returns>
        private bool IsRefreshRelationWithParentNeeded(UserDataEntity currentUserData, UserDataEntity parent)
        {
            return parent != null && currentUserData.ParentUserData != null && !currentUserData.ParentUserData.Equals(parent);
        }

        private UserDataEntity CreateNewVersion(UserDataEntity userContactEntity, DateTime now, DateTime? activeTo,
            UserDataEntity parent = null)
        {
            return new UserDataEntity
            {
                ActiveFrom = now,
                ActiveTo = activeTo,
                User = userContactEntity.User,
                VerifiedBy = userContactEntity.VerifiedBy,
                UserDataType = userContactEntity.UserDataType,
                Value = userContactEntity.Value,
                LevelOfAssurance = userContactEntity.LevelOfAssurance,
                DataSource = userContactEntity.DataSource,
                ParentUserData = parent,
            };
        }
    }
}