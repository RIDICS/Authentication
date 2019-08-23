using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.ExternalIdentity;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.UserData;
using Ridics.Authentication.DataEntities;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Exceptions;
using Ridics.Authentication.DataEntities.Models;
using Ridics.Authentication.DataEntities.Types;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.Modules.Shared.Model;
using Ridics.Authentication.Shared;
using Ridics.Core.Shared;
using Ridics.Core.Shared.Exceptions;
using Ridics.Core.Shared.Providers;

namespace Ridics.Authentication.Core.Managers
{
    public class UserManager : ManagerBase
    {
        private readonly CodeGeneratorConfiguration m_codeGeneratorConfiguration;
        private readonly CodeGeneratorManager m_codeGeneratorManager;
        private readonly ContactFormatterManager m_contactFormatterManager;
        private readonly IDateTimeStringMapper m_dateTimeStringMapper;
        private readonly IExternalIdentityResolver m_externalIdentityResolver;
        private readonly ExternalLoginProviderUoW m_externalLoginProviderUoW;
        private readonly UserUoW m_userUoW;
        private readonly ContactManager m_contactManager;
        private readonly DataSourceUoW m_dataSourceUoW;
        private IDateTimeProvider m_dateTimeProvider;

        public UserManager(UserUoW userUoW, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration, CodeGeneratorManager codeGeneratorManager,
            ExternalLoginProviderUoW externalLoginProviderUoW, IExternalIdentityResolver externalIdentityResolver,
            CodeGeneratorConfiguration codeGeneratorConfiguration, IDateTimeStringMapper dateTimeStringMapper, 
            ContactFormatterManager contactFormatterManager, ContactManager contactManager, DataSourceUoW dataSourceUoW, IDateTimeProvider dateTimeProvider) : base(
            logger, translator,
            mapper, paginationConfiguration
        )
        {
            m_userUoW = userUoW;
            m_codeGeneratorManager = codeGeneratorManager;
            m_externalLoginProviderUoW = externalLoginProviderUoW;
            m_externalIdentityResolver = externalIdentityResolver;
            m_codeGeneratorConfiguration = codeGeneratorConfiguration;
            m_dateTimeStringMapper = dateTimeStringMapper;
            m_contactFormatterManager = contactFormatterManager;
            m_contactManager = contactManager;
            m_dataSourceUoW = dataSourceUoW;
            m_dateTimeProvider = dateTimeProvider;
        }


        public DataResult<UserModel> GetUserById(int userId)
        {
            try
            {
                var user = m_userUoW.GetUserById(userId);
                var viewModel = m_mapper.Map<UserModel>(user);

                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByUsername(string username)
        {
            try
            {
                var user = m_userUoW.GetUserByUsername(username);
                var viewModel = m_mapper.Map<UserModel>(user);

                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-username"), DataResultErrorCode.UserNotExistUsername);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByConfirmedEmail(string email)
        {
            return GetUserByEmail(email, LevelsOfAssurance.ContactMinLoaToGetUserByConfirmedEmail);
        }

        private DataResult<UserModel> GetUserByEmail(string email, LevelOfAssuranceEnum minLevelOfAssurance)
        {
            var formattedEmail = m_contactFormatterManager.FormatEmail(email);

            try
            {
                var user = m_userUoW.GetUserByContact(formattedEmail, ContactTypeEnum.Email, minLevelOfAssurance);
                var viewModel = m_mapper.Map<UserModel>(user);

                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-email"), DataResultErrorCode.UserNotExistEmail);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByPhone(string phoneNumber, LevelOfAssuranceEnum minLevelOfAssurance)
        {
            var formattedPhoneNumber = m_contactFormatterManager.FormatPhoneNumber(phoneNumber);

            try
            {
                var user = m_userUoW.GetUserByContact(formattedPhoneNumber, ContactTypeEnum.Phone, minLevelOfAssurance);
                var viewModel = m_mapper.Map<UserModel>(user);

                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-phone"), DataResultErrorCode.UserNotExistPhone);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByVerificationCode(string verificationCode)
        {
            try
            {
                var user = m_userUoW.GetUserByVerificationCode(verificationCode);
                var viewModel = m_mapper.Map<UserModel>(user);

                return Success(viewModel);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-verification-code"),
                    DataResultErrorCode.UserNotExistVerificationCode);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByExternalLogin(string loginProvider, string providerKey)
        {
            try
            {
                var userEntity = m_userUoW.GetUserByExternalLogin(loginProvider, providerKey);
                var user = m_mapper.Map<UserModel>(userEntity);

                return Success(user);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(m_translator.Translate("invalid-external-login"), DataResultErrorCode.UserNotExistExternalLogin);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }

        public DataResult<UserModel> GetUserByDataType(string dataType, string dataValue)
        {
            try
            {
                var userEntity = m_userUoW.GetUserByDataType(dataType, dataValue);
                var user = m_mapper.Map<UserModel>(userEntity);

                return Success(user);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                
                return Error<UserModel>(m_translator.Translate("invalid-data-type-value"), DataResultErrorCode.UserNotExistUserData);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserModel>(e.Message);
            }
        }
        
        public DataResult<UserBasicInfoModel> GetBasicUserInfoByDataType(string dataType, string dataValue)
        {
            try
            {
                var userEntity = m_userUoW.GetBasicUserInfoByDataType(dataType, dataValue);
                var user = m_mapper.Map<UserBasicInfoModel>(userEntity);

                return Success(user);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                
                return Error<UserBasicInfoModel>(m_translator.Translate("invalid-data-type-value"), DataResultErrorCode.UserNotExistUserData);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserBasicInfoModel>(e.Message);
            }
        }

        public DataResult<IList<UserBasicInfoModel>> FindBasicUserInfosByDataType(string dataType, IList<string> dataValues)
        {
            try
            {
                var userEntities = m_userUoW.FindBasicUserInfosByDataType(dataType, dataValues);
                var userList = m_mapper.Map<IList<UserBasicInfoModel>>(userEntities);

                return Success(userList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<UserBasicInfoModel>>(e.Message);
            }
        }

        public DataResult<IList<UserModel>> FindUsers(int start, int count, string searchByName)
        {
            try
            {
                var users = m_userUoW.FindUsers(start, GetItemsOnPageCount(count), searchByName);

                var viewModelList = m_mapper.Map<IList<UserModel>>(users);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<IList<UserModel>>(e.Message);
            }
        }

        public DataResult<List<UserModel>> FindNonAuthenticationServiceUsers(int start, int count, string searchByName)
        {
            try
            {
                var users = m_userUoW.FindNonAuthenticationServiceUsers(start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<UserModel>>(users);

                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<UserModel>>(e.Message);
            }
        }

        public DataResult<int> GetUsersCount(string searchByName)
        {
            try
            {
                var usersCount = m_userUoW.GetUsersCount(searchByName);
                return Success(usersCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<int> GetNonAuthenticationServiceUsersCount(string searchByName)
        {
            try
            {
                var usersCount = m_userUoW.GetNonAuthenticationServiceUsersCount(searchByName);
                return Success(usersCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<List<UserModel>> FindNonAuthenticationUsersByRole(int roleId, int start, int count, string searchByName)
        {
            try
            {
                var users = m_userUoW.FindNonAuthenticationServiceUsersByRole(roleId, start, GetItemsOnPageCount(count), searchByName);
                var viewModelList = m_mapper.Map<List<UserModel>>(users);
                return Success(viewModelList);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<List<UserModel>>(e.Message);
            }
        }

        public DataResult<int> GetNonAuthenticationUsersByRoleCount(int roleId, string searchByName)
        {
            try
            {
                var usersCount = m_userUoW.GetNonAuthenticationServiceUsersByRoleCount(roleId, searchByName);
                return Success(usersCount);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<int> CreateUser(UserModel userModel, bool generateVerificationCode = true)
        {
            var now = m_dateTimeProvider.UtcNow;

            var userData = MapUserData(userModel.UserData);

            var user = new UserEntity
            {
                Username = userModel.Username,
                PasswordHash = userModel.PasswordHash,
                TwoFactorEnabled = userModel.TwoFactorEnabled,
                LockoutEndDateUtc = userModel.LockoutEndDateUtc,
                LockoutEnabled = userModel.LockoutEnabled,
                AccessFailedCount = userModel.AccessFailedCount,
                SecurityStamp = userModel.SecurityStamp,
                LastChange = now,
                TwoFactorProvider = userModel.TwoFactorProvider,
            };

            userData.Add(CreateMasterUserId(user));

            var formattedContacts = m_contactManager.ValidateAndFormatContactList(userModel.UserContacts, UserAction.Create);

            if (formattedContacts.HasError)
            {
                return Error<int>(formattedContacts.Error);
            }

            var userContacts = formattedContacts.Result?.Select(x => new UserContactEntity
                {
                    Type = (ContactTypeEnum)x.Type,
                    User = user,
                    Value = x.Value,
                    ConfirmCode = x.ConfirmCode
                })
                .ToList();

            var userExternalIdentityModels = m_externalIdentityResolver.Resolve(userModel);

            var userExternalIdentities = userExternalIdentityModels?.Select(x => new UserExternalIdentityEntity
                {
                    User = user,
                    ExternalIdentityType = new ExternalIdentityEntity {Name = x.ExternalIdentityType.Name},
                    ExternalIdentity = x.ExternalIdentity
                })
                .ToList();

            try
            {
                var userId = m_userUoW.CreateUser(now, user, userData.Select(x => new UserDataLoAModel
                {
                    UserData = x,
                    LevelOfAssuranceEnum = LevelOfAssuranceEnum.Low
                }).ToList(), userContacts.Select(x => new UserContactLoAModel
                {
                    UserContact = x,
                    LevelOfAssuranceEnum = LevelOfAssuranceEnum.Low
                }).ToList(), userExternalIdentities);

                if (generateVerificationCode)
                {
                    var generaCodeResult = RegenerateVerificationCode(userId);

                    if (generaCodeResult.HasError)
                    {
                        m_userUoW.DeleteUserById(userId);
                        return Error<int>(generaCodeResult.Error.Message, DataResultErrorCode.CreateUserErrorGenerateVerificationCode);
                    }
                }

                return Success(userId);
            }
            catch (SaveEntityException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-failed"), DataResultErrorCode.CreateUserError);
            }
            catch (SaveEntityException<UserDataEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-data-failed"), DataResultErrorCode.CreateUserErrorUserData);
            }
            catch (SaveContactEntityException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-contact-failed"),
                    e.ContactType == ContactTypeEnum.Email
                        ? DataResultErrorCode.CreateUserErrorUserContactEmail
                        : DataResultErrorCode.CreateUserErrorUserContactPhone);
            }
            catch (SaveEntityException<UserExternalIdentityEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-external-identity-failed"),
                    DataResultErrorCode.CreateUserErrorExternalIdentity);
            }
            catch (UserDataAlreadyExistsException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-external-identity-failed"),
                    UserDataTypeErrorCodeHelper.GetCodeForUniqueUserDataTypeException(e.DataType));
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        private UserDataEntity CreateMasterUserId(UserEntity user)
        {
            var dataSource = m_dataSourceUoW.FindDataSourceByDataSource(DataSourceEnum.System).FirstOrDefault();

            return new UserDataEntity
            {
                Value = Guid.NewGuid().ToString(),
                User = user,
                DataSource = dataSource,
                UserDataType = new UserDataTypeEntity { DataTypeValue = CustomUserDataTypes.MasterUserId },
            };
        }

        public DataResult<int> CreateUserByExternalProvider(
            string providerName,
            ExternalLoginProviderUserModel externalLoginProvider
        )
        {
            var now = m_dateTimeProvider.UtcNow;

            var user = new UserEntity
            {
                Username = GenerateUsername().Result,
                PasswordHash = "",
                TwoFactorEnabled = false,
                LockoutEndDateUtc = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                LastChange = now,
                TwoFactorProvider = null,
            };

            var userData = new List<UserDataLoAModel>();
            var userContacts = new List<UserContactLoAModel>();
            var userExternalIdentities = new List<UserExternalIdentityEntity>();

            userData.Add(new UserDataLoAModel
            {
                UserData = CreateMasterUserId(user),
                LevelOfAssuranceEnum = LevelOfAssuranceEnum.Low,
            });

            if (externalLoginProvider.Email != null)
            {
                var emailValue = externalLoginProvider.Email.Value;

                var result = m_contactManager.ValidateAndFormatContact(emailValue, ContactTypeEnumModel.Email, UserAction.Create);
                if (result.HasError)
                {
                    return Error<int>(result.Error);
                }

                userContacts.Add(new UserContactLoAModel
                {
                    UserContact = new UserContactEntity
                    {
                        Type = ContactTypeEnum.Email,
                        User = user,
                        Value = result.Result,
                    },
                    LevelOfAssuranceEnum = m_mapper.Map<LevelOfAssuranceEnum>(externalLoginProvider.Email.LevelOfAssurance)
                });
            }

            if (externalLoginProvider.PhoneNumber != null)
            {
                var phoneNumberValue = externalLoginProvider.PhoneNumber.Value;

                var result = m_contactManager.ValidateAndFormatContact(phoneNumberValue, ContactTypeEnumModel.Phone, UserAction.Create);
                if (result.HasError)
                {
                    return Error<int>(result.Error);
                }

                userContacts.Add(new UserContactLoAModel
                {
                    UserContact = new UserContactEntity
                    {
                        Type = ContactTypeEnum.Phone,
                        User = user,
                        Value = result.Result,
                    },
                    LevelOfAssuranceEnum = m_mapper.Map<LevelOfAssuranceEnum>(externalLoginProvider.PhoneNumber.LevelOfAssurance)
                });
            }

            try
            {
                var userId = m_userUoW.CreateUser(now, user, userData, userContacts, userExternalIdentities, providerName);

                return Success(userId);
            }
            catch (SaveEntityException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-failed"), DataResultErrorCode.CreateUserError);
            }
            catch (SaveContactEntityException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(m_translator.Translate("save-user-contact-failed"),
                    e.ContactType == ContactTypeEnum.Email
                        ? DataResultErrorCode.CreateUserErrorUserContactEmail
                        : DataResultErrorCode.CreateUserErrorUserContactPhone);
            }
        }

        public DataResult<UserInfoModel> RegenerateVerificationCode(int userId)
        {
            try
            {
                var generateCodeRetries = 0;

                while (generateCodeRetries <= m_codeGeneratorConfiguration.MaxGenerateVerificationCodeRetries)
                {
                    var newVerificationCode = m_codeGeneratorManager.GenerateVerificationCode();
                    var verificationCodeCreateTime = DateTime.UtcNow;

                    try
                    {
                        var entity = m_userUoW.UpdateVerificationCode(userId, newVerificationCode, verificationCodeCreateTime);
                        var result = m_mapper.Map<UserInfoModel>(entity);

                        return Success(result);
                    }
                    catch (VerficationCodeAlreadyExistsException e)
                    {
                        m_logger.LogWarning(e);
                        generateCodeRetries++;
                    }
                }


                m_logger.LogWarning("Generating verification code failed for user with id '{0}' with {1} retries",
                    userId,
                    m_codeGeneratorConfiguration.MaxGenerateVerificationCodeRetries);
                return Error<UserInfoModel>(m_translator.Translate("generate-verification-code-failed"),
                    DataResultErrorCode.GenerateVerificationCode);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserInfoModel>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserInfoModel>(e.Message);
            }
        }

        public DataResult<string> GenerateUsername()
        {
            try
            {
                var generateRetries = 0;

                while (generateRetries <= m_codeGeneratorConfiguration.MaxGenerateUsernameRetries)
                {
                    var newUsername = m_codeGeneratorManager.GenerateUsername();

                    try
                    {
                        m_userUoW.GetUserByUsername(newUsername); //check if user with generated username exists
                    }
                    catch (NoResultException<UserEntity>)
                    {
                        return Success(newUsername); //if not, return newly generated username
                    }
                }

                m_logger.LogWarning("Generating username code failed with {0} retries",
                    m_codeGeneratorConfiguration.MaxGenerateUsernameRetries);

                return Error<string>(m_translator.Translate("generate-username-failed"), DataResultErrorCode.GenerateUsername);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<string>(e.Message);
            }
        }

        public DataResult<string> GeneratePassword()
        {
            return Success(m_codeGeneratorManager.GeneratePassword());
        }

        public DataResult<bool> UpdateUser(int userId, UserModel userModel)
        {
            var userData = MapUserData(userModel.UserData);

            var now = m_dateTimeProvider.UtcNow;

            var user = new UserEntity
            {
                Username = userModel.Username,
                PasswordHash = userModel.PasswordHash,
                TwoFactorEnabled = userModel.TwoFactorEnabled,
                LockoutEndDateUtc = userModel.LockoutEndDateUtc,
                LockoutEnabled = userModel.LockoutEnabled,
                AccessFailedCount = userModel.AccessFailedCount,
                SecurityStamp = userModel.SecurityStamp,
                LastChange = now,
                TwoFactorProvider = userModel.TwoFactorProvider,
                VerificationCode = userModel.VerificationCode,
                VerificationCodeCreateTime = userModel.VerificationCodeCreateTime
            };

            var formattedContacts = m_contactManager.ValidateAndFormatContactList(userModel.UserContacts, UserAction.Update);

            if (formattedContacts.HasError)
            {
                return Error<bool>(formattedContacts.Error);
            }
            
            var userContacts = formattedContacts.Result?.Select(x => new UserContactEntity
            {
                Type = (ContactTypeEnum) x.Type,
                User = user,
                Value = x.Value,
                ConfirmCode = x.ConfirmCode
            }).ToList();

            var userExternalIdentityModels = m_externalIdentityResolver.Resolve(userModel);

            var userExternalIdentities = userExternalIdentityModels?.Select(x => new UserExternalIdentityEntity
            {
                User = user,
                ExternalIdentityType = new ExternalIdentityEntity {Name = x.ExternalIdentityType.Name},
                ExternalIdentity = x.ExternalIdentity
            }).ToList();

            try
            {
                var userDataLowLoa = userData.Select(x => new UserDataLoAModel
                {
                    UserData = x,
                    LevelOfAssuranceEnum = LevelOfAssuranceEnum.Low
                }).ToList();
                var userContractsLowLoa = userContacts.Select(x => new UserContactLoAModel
                {
                    UserContact = x,
                    LevelOfAssuranceEnum = LevelOfAssuranceEnum.Low
                }).ToList();

                m_userUoW.UpdateUser(now, userId, user, userDataLowLoa, userContractsLowLoa, userExternalIdentities);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (SaveEntityException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("save-user-failed"), DataResultErrorCode.CreateUserError);
            }
            catch (SaveEntityException<UserDataEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("save-user-data-failed"), DataResultErrorCode.CreateUserErrorUserData);
            }
            catch (SaveContactEntityException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("save-user-contact-failed"),
                    e.ContactType == ContactTypeEnum.Email
                        ? DataResultErrorCode.CreateUserErrorUserContactEmail
                        : DataResultErrorCode.CreateUserErrorUserContactPhone);
            }
            catch (SaveEntityException<UserExternalIdentityEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("save-user-external-identity-failed"),
                    DataResultErrorCode.CreateUserErrorExternalIdentity);
            }
            catch (UserDataAlreadyExistsException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("save-user-external-identity-failed"),
                    UserDataTypeErrorCodeHelper.GetCodeForUniqueUserDataTypeException(e.DataType));
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        /// <summary>
        ///     Invalidates user data -> regenerates verification code and sets Verified property of user data to false.
        ///     This method was used in UserInternalApiController.EditUserAsync before the invalidation process of userdata was moved into UserManager.UpdateUser, now is obsolete, however it can be used when need to explicitly invalidate UserData.
        /// </summary>
        /// <param name="userId">Id of user, whose data should be invalidated</param>
        /// <returns></returns>
        public DataResult<bool> InvalidateUserData(int userId)
        {
            var generaCodeResult = RegenerateVerificationCode(userId);

            if (generaCodeResult.HasError)
                return Error<bool>(generaCodeResult.Error.Message, DataResultErrorCode.CreateUserErrorGenerateVerificationCode);

            try
            {
                m_userUoW.InvalidateUserData(userId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }

            return Success(true);
        }

        public DataResult<bool> DeleteUserWithId(int userId)
        {
            try
            {
                m_userUoW.DeleteUserById(userId);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> ValidateLastChanged(int userId, string lastChanged)
        {
            UserEntity user;

            try
            {
                user = m_userUoW.GetUserById(userId);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }

            if (DateTime.TryParseExact(lastChanged, "O", null, DateTimeStyles.None, out var lastChange))
                return DateTime.Compare(lastChange, user.LastChange) >= 0
                    ? Success(true)
                    : Success(false);

            return Error<bool>();
        }

        public DataResult<bool> AddRoleToUser(int userId, int roleId)
        {
            try
            {
                m_userUoW.AddRoleToUser(userId, roleId);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AddRoleToUser(int userId, string roleName)
        {
            try
            {
                m_userUoW.AddRoleToUser(userId, roleName);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-name"), DataResultErrorCode.RoleNotExistName);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> RemoveRoleFromUser(int userId, int roleId)
        {
            try
            {
                m_userUoW.RemoveRoleFromUser(userId, roleId);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> SwitchUserRoles(int userId, IEnumerable<string> rolesToRemove, IEnumerable<string> rolesToAdd)
        {
            try
            {
                m_userUoW.SwitchUserRoles(userId, rolesToRemove, rolesToAdd);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-id"), DataResultErrorCode.RoleNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> RemoveRoleFromUser(int userId, string roleName)
        {
            try
            {
                m_userUoW.RemoveRoleFromUser(userId, roleName);
                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-role-name"), DataResultErrorCode.RoleNotExistName);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignRolesToUser(int userId, IEnumerable<int> rolesIds)
        {
            try
            {
                m_userUoW.AssignRolesToUser(userId, rolesIds);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignClaimsToUser(int id, IEnumerable<int> claimIds)
        {
            try
            {
                m_userUoW.AssignClaimsToUser(id, claimIds);
                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<int> CreateTwoFactorTokenForUser(UserModel user, string provider, string token)
        {
            var now = m_dateTimeProvider.UtcNow;

            var twoFactorLoginEntity = new TwoFactorLoginEntity
            {
                TokenProvider = provider,
                Token = token,
                CreateTime = now,
            };

            try
            {
                var result = m_userUoW.CreateTwoFactorTokenForUser(user.Id, twoFactorLoginEntity);

                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<TwoFactorLoginModel> GetTwoFactorTokenForUser(int userId, string provider)
        {
            try
            {
                var entity = m_userUoW.GetTwoFactorTokenForUser(userId, provider);
                var result = m_mapper.Map<TwoFactorLoginModel>(entity);

                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<TwoFactorLoginModel>(e.Message);
            }
        }

        public DataResult<int> CreateExternalLogin(
            int userId, string loginProvider, string providerKey
        )
        {
            var externalLoginProviderEntity = m_externalLoginProviderUoW.GetExternalLoginProviderByName(
                loginProvider
            );

            if (externalLoginProviderEntity.DisableManagingByUser)
            {
                const string errorMessage = "Edit this link is not permitted";
                m_logger.LogWarning(errorMessage);
                return Error<int>(errorMessage);
            }

            var externalLogin = new ExternalLoginEntity
            {
                Provider = externalLoginProviderEntity,
                ProviderKey = providerKey
            };

            try
            {
                var result = m_userUoW.CreateExternalLogin(userId, externalLogin);
                return Success(result);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<int>(e.Message);
            }
        }

        public DataResult<bool> DeleteExternalLoginByUser(int userId, int externalLoginId)
        {
            try
            {
                var externalLoginEntity = m_userUoW.GetExternalLoginByUserAndLoginId(
                    userId, externalLoginId
                );

                if (externalLoginEntity.Provider.DisableManagingByUser)
                {
                    const string errorMessage = "Edit this link is not permitted";
                    m_logger.LogWarning(errorMessage);
                    return Error<bool>(errorMessage);
                }

                m_userUoW.DeleteExternalLogin(userId, externalLoginId);

                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Success(false);
            }
        }

        private IList<UserDataEntity> MapUserData(IList<UserDataModel> userData)
        {
            var data = userData?.Select(x => new UserDataEntity
            {
                UserDataType = new UserDataTypeEntity {DataTypeValue = x.UserDataType.DataTypeValue},
                Value = x.Value,
                ChildrenUserData = MapUserData(x.ChildrenUserData),
                ActiveTo = x.ActiveTo,
            }).ToList();

            return data;
        }

        public DataResult<bool> SetUserDataVerified(int userId, int verifierId)
        {
            try
            {
                m_userUoW.SetUserDataVerified(userId, verifierId);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
            catch (UserDataAlreadyExistsException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("set-user-data-verified-failed"),
                    UserDataTypeErrorCodeHelper.GetCodeForUniqueUserDataTypeException(e.DataType));
            }
        }

        public DataResult<bool> EnableTwoFactorAuth(int userId, string provider = null)
        {
            try
            {
                m_userUoW.EnableTwoFactorAuth(userId, provider);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> DisableTwoFactorAuth(int userId)
        {
            try
            {
                m_userUoW.DisableTwoFactorAuth(userId);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> SavePasswordResetToken(UserModel user, string token)
        {
            try
            {
                var now = m_dateTimeProvider.UtcNow;

                m_userUoW.SavePasswordResetToken(now, user.Id, token);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignResourcePermissionsToUser(int id, IEnumerable<int> permissionIds)
        {
            try
            {
                m_userUoW.AssignResourcePermissionsToUser(id, permissionIds);

                return Success(true);
            }
            catch (NoResultException<RoleEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<bool> AssignResourcePermissionTypeActionsToUser(int id, IEnumerable<int> permissionTypeActionIds)
        {
            try
            {
                m_userUoW.AssignResourcePermissionTypeActionsToUser(id, permissionTypeActionIds);

                return Success(true);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        /// <summary>
        ///     Deletes two factor for particular user and provider. When it fails, i.e. two factor token is not found, just logs
        ///     error and returns because it is not necessary to further handle this kind of error
        /// </summary>
        /// <param name="userId">Id of user for whom 2factor token was issued</param>
        /// <param name="provider">Name of 2factor provider who issued token</param>
        public void DeleteTwoFactorTokenForUser(int userId, string provider)
        {
            try
            {
                m_userUoW.DeleteTwoFactorTokenForUser(userId, provider);
            }
            catch (NoResultException<TwoFactorLoginEntity> e)
            {
                m_logger.LogWarning(e);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
            }
        }

        /// <summary>
        ///     Deletes password reset token for specific user. When it fails, i.e. user is not found, just logs error and returns
        ///     because it is not necessary to further handle this kind of error
        /// </summary>
        /// <param name="userId">Id of user</param>
        public void DeletePasswordResetToken(int userId)
        {
            try
            {
                m_userUoW.DeletePasswordResetToken(userId);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
            }
        }

        public DataResult<bool> SetTwoFactorProvider(int userId, string twoFactorProvider)
        {
            try
            {
                m_userUoW.SetTwoFactorProvider(userId, twoFactorProvider);
                return Success(true);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<bool>(e.Message);
            }
        }

        public DataResult<UserBasicInfoModel> GetBasicUserInfoByVerifiedDataType(string dataType, string dataValue)
        {
            try
            {
                var userEntity = m_userUoW.GetBasicUserInfoByVerifiedDataType(dataType, dataValue);
                var user = m_mapper.Map<UserBasicInfoModel>(userEntity);

                return Success(user);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserBasicInfoModel>(m_translator.Translate("invalid-data-type-value"), DataResultErrorCode.UserNotExistUserData);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserBasicInfoModel>(e.Message);
            }
        }

        public DataResult<UserBasicInfoModel> GetBasicUserInfoById(int id)
        {
            try
            {
                var userEntity = m_userUoW.GetBasicUserInfoById(id);
                var user = m_mapper.Map<UserBasicInfoModel>(userEntity);

                return Success(user);
            }
            catch (NoResultException<UserEntity> e)
            {
                m_logger.LogWarning(e);
                return Error<UserBasicInfoModel>(m_translator.Translate("invalid-user-id"), DataResultErrorCode.UserNotExistId);
            }
            catch (DatabaseException e)
            {
                m_logger.LogWarning(e);
                return Error<UserBasicInfoModel>(e.Message);
            }
        }
    }
}