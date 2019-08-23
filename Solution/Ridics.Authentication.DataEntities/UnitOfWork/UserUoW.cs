using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using ImTools;
using NHibernate;
using Ridics.Authentication.DataEntities.Caching;
using Ridics.Authentication.DataEntities.DataTypes;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Models;
using Ridics.Authentication.DataEntities.Proxies;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Authentication.DataEntities.Types;
using Ridics.Authentication.DataEntities.Utils;
using Ridics.Authentication.DataEntities.Validators;
using Ridics.Core.DataEntities.Shared.UnitOfWorks;
using Ridics.Core.Shared.Providers;

namespace Ridics.Authentication.DataEntities.UnitOfWork
{
    public class UserUoW : UnitOfWorkBase
    {
        private readonly UserRepository m_userRepository;
        private readonly RoleRepository m_roleRepository;
        private readonly ClaimRepository m_claimRepository;
        private readonly UserDataTypeRepository m_userDataTypeRepository;
        private readonly ExternalIdentityRepository m_externalIdentityRepository;
        private readonly UserContactVersioningProxy m_userContactRepository;
        private readonly ResourcePermissionRepository m_resourcePermissionRepository;
        private readonly ResourcePermissionTypeActionRepository m_resourcePermissionTypeActionRepository;
        private readonly UserDataVersioningProxy m_userDataVersioningProxy;
        private readonly LevelOfAssuranceRepository m_levelOfAssuranceRepository;
        private readonly DataSourceRepository m_dataSourceRepository;
        private readonly ExternalLoginProviderRepository m_externalLoginProviderRepository;
        private readonly IDateTimeProvider m_dateTimeProvider;
        private readonly IUserDataValidatorManager m_userDataValidatorManager;
        private readonly UserDataStructureConvertor m_userDataStructureConvertor;

        public UserUoW(
            ISessionManager sessionManager,
            UserRepository userRepository,
            RoleRepository roleRepository, 
            ClaimRepository claimRepository, 
            UserDataTypeRepository userDataTypeRepository,
            ExternalIdentityRepository externalIdentityRepository, 
            UserContactVersioningProxy userContactRepository,
            ResourcePermissionRepository resourcePermissionRepository,
            ResourcePermissionTypeActionRepository resourcePermissionTypeActionRepository,
            UserDataVersioningProxy userDataVersioningProxy,
            LevelOfAssuranceRepository levelOfAssuranceRepository,
            DataSourceRepository dataSourceRepository,
            ExternalLoginProviderRepository externalLoginProviderRepository,
            IDateTimeProvider dateTimeProvider,
            IUserDataValidatorManager userDataValidatorManager, 
            UserDataStructureConvertor userDataStructureConvertor) : base(sessionManager)
        {
            m_userRepository = userRepository;
            m_roleRepository = roleRepository;
            m_claimRepository = claimRepository;
            m_userDataTypeRepository = userDataTypeRepository;
            m_externalIdentityRepository = externalIdentityRepository;
            m_userContactRepository = userContactRepository;
            m_resourcePermissionRepository = resourcePermissionRepository;
            m_resourcePermissionTypeActionRepository = resourcePermissionTypeActionRepository;
            m_userDataVersioningProxy = userDataVersioningProxy;
            m_levelOfAssuranceRepository = levelOfAssuranceRepository;
            m_dataSourceRepository = dataSourceRepository;
            m_externalLoginProviderRepository = externalLoginProviderRepository;
            m_dateTimeProvider = dateTimeProvider;
            m_userDataValidatorManager = userDataValidatorManager;
            m_userDataStructureConvertor = userDataStructureConvertor;
        }

        [Transaction]
        public virtual IList<UserEntity> FindUsers(int start, int count, string searchByName)
        {
            var users = m_userRepository.FindUsers(start, count, UserType.All, searchByName);

            HydrateUserListWithUserDataAndContacts(users);
            return users;
        }

        [Transaction]
        public virtual IList<UserEntity> FindNonAuthenticationServiceUsers(int start, int count, string searchByName)
        {
            var users = m_userRepository.FindUsers(start, count, UserType.NonAuthenticationService, searchByName);

            HydrateUserListWithUserDataAndContacts(users);
            return users;
        }

        [Transaction]
        public virtual int GetUsersCount(string searchByName)
        {
            var usersCount = m_userRepository.GetUsersCount(UserType.All, searchByName);

            return usersCount;
        }

        [Transaction]
        public virtual int GetNonAuthenticationServiceUsersCount(string searchByName)
        {
            var usersCount = m_userRepository.GetUsersCount(UserType.NonAuthenticationService, searchByName);

            return usersCount;
        }

        [Transaction]
        public virtual IList<UserEntity> FindNonAuthenticationServiceUsersByRole(int roleId, int start, int count, string searchByName)
        {
            var users = m_userRepository.FindUsersByRole(UserType.NonAuthenticationService, roleId, start, count, searchByName);

            HydrateUserListWithUserDataAndContacts(users);
            return users;
        }

        [Transaction]
        public virtual int GetNonAuthenticationServiceUsersByRoleCount(int roleId, string searchByName)
        {
            var usersCount = m_userRepository.GetUsersByRoleCount(UserType.NonAuthenticationService, roleId, searchByName);
            return usersCount;
        }

        [Transaction]
        public virtual int CreateUser(DateTime now,
            UserEntity user,
            IList<UserDataLoAModel> userData,
            IList<UserContactLoAModel> userContacts,
            IList<UserExternalIdentityEntity> userExternalIdentities,
            string externalProviderName = null
        )
        {
            m_userDataValidatorManager.ValidateUserData(
                userData.Select(x => x.UserData).ToList()
            );

            var userId = m_userRepository.CreateUser(user);

            var userEntity = m_userRepository.Load<UserEntity>(userId);

            UpdateUserData(now, userEntity, userData, UserAction.Create, externalProviderName);

            UpdateUserContacts(now, userEntity, userContacts, UserAction.Create, externalProviderName);

            UpdateUserExternalIds(userEntity, userExternalIdentities);

            return userId;
        }

        [Transaction]
        public virtual UserEntity GetUserById(int id)
        {
            var user = m_userRepository.GetUserById(id);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual UserEntity GetUserByUsername(string username)
        {
            var user = m_userRepository.GetUserByUsername(username);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual void DeleteUserById(int id)
        {
            var user = m_userRepository.Load<UserEntity>(id);

            try
            {
                m_userRepository.Delete(user);
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<UserEntity>();
            }
        }

        [Transaction]
        public virtual void UpdateUser(DateTime now,
            int userId, UserEntity user,
            IList<UserDataLoAModel> userData,
            IList<UserContactLoAModel> userContacts,
            IList<UserExternalIdentityEntity> userExternalIdentities
        )
        {
            var userEntity = m_userRepository.GetUserById(userId);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            m_userDataValidatorManager.ValidateUserData(
                userData.Select(x => x.UserData).ToList(), 
                userEntity.Id
            );

            userEntity.TwoFactorEnabled = user.TwoFactorEnabled;
            userEntity.TwoFactorProvider = user.TwoFactorProvider;

            userEntity.LastChange = now;
            userEntity.PasswordHash =
                string.IsNullOrEmpty(user.PasswordHash)
                    ? userEntity.PasswordHash
                    : user.PasswordHash; //HACK PREVENT UPDATE PASSWORD THIS WAY!!!

            if (!string.IsNullOrEmpty(user.VerificationCode) && userEntity.VerificationCode != user.VerificationCode)
            {
                userEntity.VerificationCode = user.VerificationCode;
                userEntity.VerificationCodeCreateTime = now;
            }
            else if (string.IsNullOrEmpty(user.VerificationCode))
            {
                userEntity.VerificationCode = null;
                userEntity.VerificationCodeCreateTime = null;
            }

            userEntity.SecurityStamp = user.SecurityStamp;
            userEntity.AccessFailedCount = user.AccessFailedCount;
            userEntity.LockoutEndDateUtc = user.LockoutEndDateUtc;

            // Not updated properties:
            //userEntity.Id;
            //userEntity.Username;

            //TODO investigate refactoring
            m_userRepository.Update(userEntity);

            UpdateUserData(now, userEntity, userData, UserAction.Update);

            UpdateUserContacts(now, userEntity, userContacts, UserAction.Update);

            UpdateUserExternalIds(userEntity, userExternalIdentities);
        }

        private void UpdateUserExternalIds(UserEntity userEntity, IList<UserExternalIdentityEntity> userExternalIdentities)
        {
            var userExternalIdDict = userEntity.ExternalIdentities?.ToDictionary(x => x.ExternalIdentityType.Name);
            var userExternalIdTypesDict = m_externalIdentityRepository.FindAllExternalIdentity().ToDictionary(x => x.Name);
            var userExternalIdsToUpdate = new List<UserExternalIdentityEntity>();

            foreach (var userExternalIdentity in userExternalIdentities)
            {
                if (userExternalIdDict != null && userExternalIdDict.ContainsKey(userExternalIdentity.ExternalIdentityType.Name))
                {
                    var userExternalIdsToUpdateEntity = userExternalIdDict[userExternalIdentity.ExternalIdentityType.Name];
                    userExternalIdsToUpdateEntity.ExternalIdentity = userExternalIdentity.ExternalIdentity;
                    userExternalIdsToUpdate.Add(userExternalIdsToUpdateEntity);
                }
                else
                {
                    userExternalIdentity.User = userEntity;
                    userExternalIdentity.ExternalIdentityType = userExternalIdTypesDict[userExternalIdentity.ExternalIdentityType.Name];
                    userExternalIdsToUpdate.Add(userExternalIdentity);
                }
            }

            m_userRepository.UpdateUserExternalIdentities(userExternalIdsToUpdate);
        }

        private void UpdateUserContacts(DateTime now, 
            UserEntity userEntity,
            IList<UserContactLoAModel> userContacts,
            UserAction action,
            string externalProviderName = null
        )
        {
            var userContactsDict = m_userContactRepository.GetUserContacts(userEntity.Id)?.ToDictionary(x => x.Type);
            var userContactsToUpdate = new List<UserContactEntity>();

            var levelOfAssuranceCache = new LevelOfAssuranceCache(m_levelOfAssuranceRepository);
            var dataSourceCache = new DataSourceCache(m_dataSourceRepository, m_externalLoginProviderRepository);

            foreach (var userContactModel in userContacts)
            {
                var userContact = userContactModel.UserContact;

                if (userContactsDict != null && userContactsDict.ContainsKey(userContact.Type))
                {
                    var userContactEntity = userContactsDict[userContact.Type];

                    if (!userContact.Value.Equals(userContactEntity.Value))
                    {
                        userContactEntity.LevelOfAssurance = levelOfAssuranceCache.GetByEnum(userContactModel.LevelOfAssuranceEnum);
                        userContactEntity.DataSource = dataSourceCache.GetByExternalProvider(externalProviderName);
                    }

                    userContactEntity.Value = userContact.Value;

                    if (
                        userContact.ConfirmCode != null
                        && !userContact.ConfirmCode.Equals(userContactEntity.ConfirmCode)
                    )
                    {
                        userContactEntity.ConfirmCode = userContact.ConfirmCode;
                        userContactEntity.ConfirmCodeChangeTime = now;
                    }

                    userContactsToUpdate.Add(userContactEntity);
                }
                else
                {
                    // expect this mean it is new "row"

                    userContact.User = userEntity;
                    userContact.ConfirmCodeChangeTime = now;

                    userContact.LevelOfAssurance = levelOfAssuranceCache.GetByEnum(userContactModel.LevelOfAssuranceEnum);
                    userContact.DataSource = dataSourceCache.GetByExternalProvider(externalProviderName);

                    userContactsToUpdate.Add(userContact);
                }
            }

            switch (action)
            {
                case UserAction.Create:
                    m_userContactRepository.CreateUserContacts(userContactsToUpdate);
                    break;
                case UserAction.Update:
                    m_userContactRepository.UpdateUserContacts(userContactsToUpdate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void UpdateUserData(
            DateTime now,
            UserEntity userEntity,
            IList<UserDataLoAModel> userData,
            UserAction action,
            string externalProviderName = null
        )
        {
            var currentUserDataSet = m_userDataVersioningProxy.FindUserDataWithTreeStructure(userEntity.Id);
            var userDataDict = m_userDataStructureConvertor.GetFlatUserDataList(currentUserDataSet)
                ?.ToDictionary(x => x.UserDataType.DataTypeValue);
            var userDataTypesDict = m_userDataTypeRepository.GetAllUserDataTypes().ToDictionary(x => x.DataTypeValue);

            var userDataToUpdate = new List<UserDataEntity>();

            HydrateUserData(now,
                userEntity, userData, userDataToUpdate, userDataDict, userDataTypesDict, externalProviderName
            );

            switch (action)
            {
                case UserAction.Create:
                    m_userDataVersioningProxy.CreateUserData(userDataToUpdate);
                    break;
                case UserAction.Update:
                    m_userDataVersioningProxy.UpdateUserData(userDataToUpdate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
            
        }

        private void HydrateUserData(
            DateTime now,
            UserEntity userEntity,
            IList<UserDataLoAModel> userData,
            IList<UserDataEntity> userDataToUpdate,
            IDictionary<string, UserDataEntity> userDataDict,
            IDictionary<string, UserDataTypeEntity> userDataTypesDict,
            string externalProviderName,
            UserDataEntity parentEntity = null
        )
        {
            if (userData == null)
            {
                return;
            }

            var levelOfAssuranceCache = new LevelOfAssuranceCache(m_levelOfAssuranceRepository);

            var dataSource = Lazy.Of(() =>
                {
                    if (string.IsNullOrEmpty(externalProviderName))
                    {
                        return m_dataSourceRepository.GetDataSourceByDataSource(
                            DataSourceEnum.User
                        );
                    }

                    var externalLoginProviderEntity = m_externalLoginProviderRepository.GetExternalLoginProviderByName(
                        externalProviderName
                    );

                    return m_dataSourceRepository.GetDataSourceByDataSource(
                        DataSourceEnum.ExternalLoginProvider,
                        externalLoginProviderEntity.Id
                    );
                }
            );

            foreach (var dataEntityModel in userData)
            {
                var dataEntity = dataEntityModel.UserData;

                if (userDataDict != null && userDataDict.ContainsKey(dataEntity.UserDataType.DataTypeValue))
                {
                    var userDataEntity = userDataDict[dataEntity.UserDataType.DataTypeValue];

                    if (dataEntity.Value != userDataEntity.Value)
                    {
                        userDataEntity.LevelOfAssurance = levelOfAssuranceCache.GetByEnum(dataEntityModel.LevelOfAssuranceEnum);
                        userDataEntity.DataSource = dataEntityModel.UserData.DataSource ?? dataSource.Value; //UserData generated by system, e.g. MasterUserId, has DataSource already set
                    }

                    userDataEntity.Value = dataEntity.Value;

                    if (dataEntity.ActiveTo == null || dataEntity.ActiveTo > now)
                    {
                        userDataEntity.ActiveTo = dataEntity.ActiveTo;
                    }
                    else
                    {
                        throw new InvalidOperationException("ActiveTo property of new UserData version must be null or greater than UtcNow");
                    }
                    
                    userDataEntity.ParentUserData = parentEntity;

                    userDataToUpdate.Add(userDataEntity);

                    HydrateUserData(now,
                        userEntity, dataEntity.ChildrenUserData?.Select(x => new UserDataLoAModel
                        {
                            UserData = x,
                            LevelOfAssuranceEnum = dataEntityModel.LevelOfAssuranceEnum
                        }).ToList(), userDataToUpdate,
                        userDataDict, userDataTypesDict, externalProviderName, userDataEntity
                    );
                }
                else
                {
                    // expect this mean it is new "row"

                    dataEntity.User = userEntity;
                    dataEntity.UserDataType = userDataTypesDict[dataEntity.UserDataType.DataTypeValue];
                    dataEntity.ParentUserData = parentEntity;

                    dataEntity.LevelOfAssurance = levelOfAssuranceCache.GetByEnum(dataEntityModel.LevelOfAssuranceEnum);
                    dataEntity.DataSource = dataEntityModel.UserData.DataSource ?? dataSource.Value;

                    userDataToUpdate.Add(dataEntity);

                    HydrateUserData(now,
                        userEntity, dataEntity.ChildrenUserData?.Select(x => new UserDataLoAModel
                        {
                            UserData = x,
                            LevelOfAssuranceEnum = dataEntityModel.LevelOfAssuranceEnum
                        }).ToList(), userDataToUpdate,
                        userDataDict, userDataTypesDict, externalProviderName, dataEntity
                    );
                }
            }
        }

        [Transaction]
        public virtual int CreateExternalLogin(
            int userId,
            ExternalLoginEntity externalLogin,
            IEnumerable<string> rolesToAdd
        )
        {
            var userEntity = m_userRepository.Load<UserEntity>(userId);

            externalLogin.User = userEntity;

            var result = (int) m_userRepository.Create(externalLogin);

            foreach (var roleToAdd in rolesToAdd)
            {
                AddRoleToUserInternal(externalLogin.User.Id, roleToAdd);
            }

            return result;
        }

        public int CreateExternalLogin(int userId, ExternalLoginEntity externalLogin)
        {
            return CreateExternalLogin(userId, externalLogin, new List<string>());
        }

        [Transaction]
        public virtual ExternalLoginEntity GetExternalLoginByUserAndLoginId(int userId, int externalLoginId)
        {
            var result = m_userRepository.GetExternalLoginByUserAndLoginId(userId, externalLoginId);

            if (result == null)
            {
                throw new NoResultException<ExternalLoginEntity>();
            }

            return result;
        }

        [Transaction]
        public virtual void DeleteExternalLogin(
            int userId, int externalLoginId, IEnumerable<string> rolesToRemove
        )
        {
            var externalLogin = m_userRepository.GetExternalLoginByUserAndLoginId(userId, externalLoginId);

            foreach (var roleToRemove in rolesToRemove)
            {
                RemoveRoleFromUserInternal(externalLogin.User.Id, roleToRemove);
            }

            m_userRepository.Delete(externalLogin);
        }

        public virtual void DeleteExternalLogin(
            int userId, int externalLoginId
        )
        {
            DeleteExternalLogin(userId, externalLoginId, new List<string>());
        }

        [Transaction]
        public virtual UserEntity GetUserByExternalLogin(string loginProvider, string providerKey)
        {
            var user = m_userRepository.GetUserByExternalProvider(loginProvider, providerKey);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual IList<ExternalLoginEntity> FindAllExternalLoginByProvider(int externalLoginProviderId)
        {
            var externalLogins = m_userRepository.FindAllExternalLoginByProvider(externalLoginProviderId);

            return externalLogins;
        }

        [Transaction]
        public virtual void AddRoleToUser(int id, int roleId)
        {
            AddRoleToUserInternal(id, roleId);
        }

        private void AddRoleToUserInternal(int id, int roleId)
        {
            var roleEntity = m_roleRepository.FindRoleById(roleId);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            var userEntity = m_userRepository.GetUserById(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.Roles = userEntity.Roles ?? new HashSet<RoleEntity>();

            userEntity.Roles.Add(roleEntity);

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void AddRoleToUser(int id, string roleName)
        {
            AddRoleToUserInternal(id, roleName);
        }

        private void AddRoleToUserInternal(int id, string roleName)
        {
            var roleEntity = m_roleRepository.GetRoleByName(roleName);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            var userEntity = m_userRepository.GetUserById(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.Roles = userEntity.Roles ?? new HashSet<RoleEntity>();

            userEntity.Roles.Add(roleEntity);

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void RemoveRoleFromUser(int id, int roleId)
        {
            RemoveRoleFromUserInternal(id, roleId);
        }

        private void RemoveRoleFromUserInternal(int id, int roleId)
        {
            var roleEntity = m_roleRepository.FindRoleById(roleId);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            var userEntity = m_userRepository.GetUserById(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.Roles = userEntity.Roles ?? new HashSet<RoleEntity>();

            userEntity.Roles.Remove(roleEntity);

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void SwitchUserRoles(int userId, IEnumerable<string> rolesToRemove, IEnumerable<string> rolesToAdd)
        {
            foreach (var roleToAdd in rolesToAdd)
            {
                AddRoleToUserInternal(userId, roleToAdd);
            }

            foreach (var roleToRemove in rolesToRemove)
            {
                RemoveRoleFromUserInternal(userId, roleToRemove);
            }
        }

        [Transaction]
        public virtual void RemoveRoleFromUser(int id, string roleName)
        {
            RemoveRoleFromUserInternal(id, roleName);
        }

        private void RemoveRoleFromUserInternal(int id, string roleName)
        {
            var roleEntity = m_roleRepository.GetRoleByName(roleName);

            if (roleEntity == null)
            {
                throw new NoResultException<RoleEntity>();
            }

            var userEntity = m_userRepository.GetUserById(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.Roles = userEntity.Roles ?? new HashSet<RoleEntity>();

            userEntity.Roles.Remove(roleEntity);

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void AssignRolesToUser(int id, IEnumerable<int> rolesIds)
        {
            var userEntity = m_userRepository.FindById<UserEntity>(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.Roles = new HashSet<RoleEntity>(m_roleRepository.GetRolesById(rolesIds));

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void AssignClaimsToUser(int id, IEnumerable<int> claimIds)
        {
            var userEntity = m_userRepository.FindById<UserEntity>(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.UserClaims = new HashSet<ClaimEntity>(m_claimRepository.GetClaimsById(claimIds));

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual UserEntity GetUserByContact(string contactValue, ContactTypeEnum contactType, LevelOfAssuranceEnum minLevelOfAssurance)
        {
            var user = m_userRepository.GetUserByValidContact(contactValue, contactType, minLevelOfAssurance);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual UserEntity GetUserByDataType(string dataType, string dataValue)
        {
            var now = m_dateTimeProvider.UtcNow;
            var user = m_userRepository.GetUserByDataType(dataType, dataValue, now);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual UserEntity GetBasicUserInfoByDataType(string dataType, string dataValue)
        {
            var now = m_dateTimeProvider.UtcNow;
            var user = m_userRepository.GetUserByDataType(dataType, dataValue, now, false);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);

            return user;
        }

        [Transaction]
        public virtual IList<UserEntity> FindBasicUserInfosByDataType(string dataType, IList<string> dataValues)
        {
            var now = m_dateTimeProvider.UtcNow;
            var userList = m_userRepository.FindUsersByDataType(dataType, dataValues, now, false);

            HydrateUserListWithUserDataAndContacts(userList);

            return userList;
        }

        [Transaction]
        public virtual UserEntity GetUserByVerificationCode(string verificationCode)
        {
            var user = m_userRepository.GetUserByVerificationCode(verificationCode);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);
            return user;
        }

        [Transaction]
        public virtual int CreateTwoFactorTokenForUser(int userId, TwoFactorLoginEntity twoFactorLoginEntity)
        {
            try
            {
                var twoFactorLogin =
                    m_userRepository.GetTwoFactorTokenForUser(userId, twoFactorLoginEntity.TokenProvider);

                if (twoFactorLogin != null)
                {
                    m_userRepository.Delete(twoFactorLogin);
                    GetSession().Flush();
                }

                var user = m_userRepository.FindById<UserEntity>(userId);

                twoFactorLoginEntity.User = user ?? throw new NoResultException<UserEntity>();

                var result = (int) m_userRepository.Create(twoFactorLoginEntity);

                return result;
            }
            catch (ObjectNotFoundException)
            {
                throw new NoResultException<UserEntity>();
            }
        }

        [Transaction]
        public virtual TwoFactorLoginEntity GetTwoFactorTokenForUser(int userId, string provider)
        {
            var result = m_userRepository.GetTwoFactorTokenForUser(userId, provider);

            return result;
        }

        [Transaction]
        public virtual UserEntity UpdateVerificationCode(int userId, string verificationCode, DateTime verificationCodeCreateTime)
        {
            var verCodeUser = m_userRepository.GetUserByVerificationCode(verificationCode);

            if (verCodeUser != null)
            {
                throw new VerficationCodeAlreadyExistsException(verificationCode);
            }


            var userEntity = m_userRepository.GetUserById(userId);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.VerificationCode = verificationCode;
            userEntity.VerificationCodeCreateTime = verificationCodeCreateTime;

            m_userRepository.Update(userEntity);

            HydrateUserWithUserDataAndContacts(userEntity);
            return userEntity;
        }
        
        [Transaction]
        public virtual void InvalidateUserData(int userId)
        {
            var userData = m_userDataVersioningProxy.FindUserDataWithTreeStructure(userId);
            
            var levelOfAssurance = m_levelOfAssuranceRepository.GetLevelOfAssuranceByName(LevelsOfAssurance.UserDataLoaAfterInvalidation);

            foreach (var userDataEntity in userData)
            {
                SetTreeStructureUserDataVerificationStatus(userDataEntity, null, levelOfAssurance);
            }

            m_userDataVersioningProxy.UpdateUserData(userData);
        }

        [Transaction]
        public virtual void SetUserDataVerified(int userId, int verifierId)
        {
            var userData = m_userDataVersioningProxy.FindUserDataWithTreeStructure(userId);

            m_userDataValidatorManager.ValidateUserData(userData, userId);

            var verifier = m_userRepository.Load<UserEntity>(verifierId);

            var levelOfAssurance = m_levelOfAssuranceRepository.GetLevelOfAssuranceByName(LevelsOfAssurance.UserDataLoaAfterVerification);

            foreach (var userDataEntity in userData)
            {
                SetTreeStructureUserDataVerificationStatus(userDataEntity, verifier, levelOfAssurance);
            }

            m_userDataVersioningProxy.UpdateUserData(userData);

            ResetVerificationCode(userId);
        }

        private void ResetVerificationCode(int userId)
        {
            var user = m_userRepository.Load<UserEntity>(userId);
            user.VerificationCode = null;
            m_userRepository.Update(user);
        }

        private void SetTreeStructureUserDataVerificationStatus(UserDataEntity userData, UserEntity verifier, LevelOfAssuranceEntity levelOfAssurance)
        {
            userData.VerifiedBy = verifier;
            userData.LevelOfAssurance = levelOfAssurance;

            if (userData.ChildrenUserData != null)
            {
                foreach (var childrenUserData in userData.ChildrenUserData)
                {
                    SetTreeStructureUserDataVerificationStatus(childrenUserData, verifier, levelOfAssurance);
                }
            }
        }

        [Transaction]
        public virtual void EnableTwoFactorAuth(int userId, string provider)
        {
            var user = m_userRepository.GetUserById(userId);

            user.TwoFactorEnabled = true;

            if (!string.IsNullOrEmpty(provider))
            {
                user.TwoFactorProvider = provider;
            }

            m_userRepository.Update(user);
        }

        [Transaction]
        public virtual void DisableTwoFactorAuth(int userId)
        {
            var user = m_userRepository.GetUserById(userId);

            user.TwoFactorEnabled = false;

            m_userRepository.Update(user);
        }

        [Transaction]
        public virtual void SavePasswordResetToken(DateTime now, int userId, string token)
        {
            var user = m_userRepository.GetUserById(userId);

            user.PasswordResetToken = token;
            user.PasswordResetTokenCreateTime = now;

            m_userRepository.Update(user);
        }

        [Transaction]
        public virtual void AssignResourcePermissionsToUser(int id, IEnumerable<int> permissionIds)
        {
            var userEntity = m_userRepository.FindById<UserEntity>(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.ResourcePermissions =
                new HashSet<ResourcePermissionEntity>(m_resourcePermissionRepository.GetPermissionsById(permissionIds));

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void AssignResourcePermissionTypeActionsToUser(int id, IEnumerable<int> permissionTypeActionIds)
        {
            var userEntity = m_userRepository.FindById<UserEntity>(id);

            if (userEntity == null)
            {
                throw new NoResultException<UserEntity>();
            }

            userEntity.ResourcePermissionTypeActions =
                new HashSet<ResourcePermissionTypeActionEntity>(
                    m_resourcePermissionTypeActionRepository.GetPermissionTypeActionsById(permissionTypeActionIds));

            m_userRepository.Update(userEntity);
        }

        [Transaction]
        public virtual void DeleteTwoFactorTokenForUser(int userId, string provider)
        {
            var result = m_userRepository.GetTwoFactorTokenForUser(userId, provider);

            if (result == null)
            {
                throw new NoResultException<TwoFactorLoginEntity>();
            }

            m_userRepository.Delete(result);
        }

        [Transaction]
        public virtual void DeletePasswordResetToken(int userId)
        {
            var user = m_userRepository.GetUserById(userId);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            user.PasswordResetToken = null;
            user.PasswordResetTokenCreateTime = null;

            m_userRepository.Update(user);
        }

        [Transaction]
        public virtual void SetTwoFactorProvider(int userId, string twoFactorProvider)
        {
            var user = m_userRepository.GetUserById(userId);

            if (!string.IsNullOrEmpty(twoFactorProvider))
            {
                user.TwoFactorProvider = twoFactorProvider;
            }

            m_userRepository.Update(user);
        }

        [Transaction]
        public virtual UserEntity GetBasicUserInfoByVerifiedDataType(string dataType, string dataValue)
        {
            var now = m_dateTimeProvider.UtcNow;
            var user = m_userRepository.GetUserByDataTypeWithMinLoa(dataType, dataValue, now, LevelsOfAssurance.UserDataMinLoaToGetUserByVerifiedUserData, false);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);

            return user;
        }

        [Transaction]
        public virtual UserEntity GetBasicUserInfoById(int userId)
        {
            var user = m_userRepository.FindById<UserEntity>(userId);

            if (user == null)
            {
                throw new NoResultException<UserEntity>();
            }

            HydrateUserWithUserDataAndContacts(user);

            return user;
        }

        private void HydrateUserListWithUserDataAndContacts(IList<UserEntity> users)
        {
            foreach (var user in users)
            {
                HydrateUserWithUserDataAndContacts(user);
            }
        }

        private void HydrateUserWithUserDataAndContacts(UserEntity user)
        {
            HydrateUserWithUserData(user);
            HydrateUserWithContacts(user);
        }

        private void HydrateUserWithUserData(UserEntity user)
        {
            user.UserData = new HashSet<UserDataEntity>(m_userDataVersioningProxy.FindUserDataWithTreeStructure(user.Id));
        }

        private void HydrateUserWithContacts(UserEntity user)
        {
            user.UserContacts = new HashSet<UserContactEntity>(m_userContactRepository.GetUserContacts(user.Id));
        }
    }
}