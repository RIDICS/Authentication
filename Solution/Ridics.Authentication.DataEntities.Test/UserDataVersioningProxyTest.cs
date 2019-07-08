using System;
using System.Collections.Generic;
using System.Linq;
using DryIoc.Facilities.NHibernate;
using Moq;
using Ridics.Authentication.DataEntities.Comparer;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Authentication.DataEntities.Providers;
using Ridics.Authentication.DataEntities.Proxies;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Authentication.DataEntities.Utils;
using Ridics.Core.Shared.Providers;
using Ridics.Core.Test.Shared.Database;
using Xunit;

namespace Ridics.Authentication.DataEntities.Test
{
    public class UserDataVersioningProxyTest
    {
        private const string FirstNameDataType = "FirstName";
        private const string LastNameDataType = "LastName";
        private const string TimeLimitedDataType = "TimeLimitedData";
        private const string NameDataType = "Name";
        private const string Username = "TestUsername";
        private const string FirstName = "Tester";
        private const string LastName = "Testerovic";
        private const string OldFirstName = "Test";
        private const string NewFirstName = "New Test";
        private const LevelOfAssuranceEnum MediumLoa = LevelOfAssuranceEnum.Substantial;
        private const LevelOfAssuranceEnum LowLoa = LevelOfAssuranceEnum.Low;

        private static readonly DateTime m_activeFrom = new DateTime(2019, 1, 1, 0, 0, 0);
        private static readonly DateTime? m_activeTo = new DateTime(2019, 2, 1, 0, 0, 0);

        private readonly UserDataVersioningProxy m_dataVersioningProxy;
        private readonly ISessionManager m_sessionManager;

        private readonly UserDataRepository m_userDataRepository;

        private readonly Mock<IDateTimeProvider> m_dateTimeProviderMock;

        private readonly UserEntity m_testUser;
        private readonly LevelOfAssuranceEntity m_lowLevelOfAssurance;
        private readonly UserDataTypeEntity m_firstNameDataType;
        private readonly UserDataTypeEntity m_lastNameDataType;
        private readonly DataSourceEntity m_userDataSource;

        public static readonly List<UserDataEntity> CreateUserDataData = new List<UserDataEntity>
        {
            new UserDataEntity
            {
                ActiveFrom = DateTime.UtcNow.AddHours(-1), ActiveTo = null, DataSource = null, ChildrenUserData = null,
                LevelOfAssurance = null, ParentUserData = null, User = null,
                UserDataType = new UserDataTypeEntity {DataTypeValue = "FirstName"}, Value = "Tester new ", VerifiedBy = null,
            },
            new UserDataEntity
            {
                ActiveFrom = DateTime.UtcNow.AddDays(-1), ActiveTo = DateTime.UtcNow.AddHours(-1), DataSource = null,
                ChildrenUserData = null, LevelOfAssurance = null, ParentUserData = null, User = null,
                UserDataType = new UserDataTypeEntity {DataTypeValue = "FirstName"}, Value = "Tester", VerifiedBy = null,
            },
            new UserDataEntity
            {
                ActiveFrom = DateTime.UtcNow, ActiveTo = null, DataSource = null, ChildrenUserData = null, LevelOfAssurance = null,
                ParentUserData = null, User = null, UserDataType = new UserDataTypeEntity {DataTypeValue = "LastName"},
                Value = "Testerovic", VerifiedBy = null,
            },
            new UserDataEntity
            {
                ActiveFrom = DateTime.UtcNow, ActiveTo = null, DataSource = null, ChildrenUserData = null, LevelOfAssurance = null,
                ParentUserData = null, User = null, UserDataType = new UserDataTypeEntity {DataTypeValue = "DateOfBirth"},
                Value = "01.01.1990", VerifiedBy = null,
            },
        };

        public static readonly TheoryData<GetUserDataParams> GetUserDataListDataFail = new TheoryData<GetUserDataParams>
        {
            new GetUserDataParams {Value = OldFirstName, Type = FirstNameDataType, LevelOfAssurance = MediumLoa},
            new GetUserDataParams {Value = FirstName, Type = LastName, LevelOfAssurance = MediumLoa},
            new GetUserDataParams {Value = FirstName, Type = FirstNameDataType, LevelOfAssurance = LevelOfAssuranceEnum.High},
        };

        public static readonly TheoryData<UpdateUserDataParams> UpdateUserDataData = new TheoryData<UpdateUserDataParams>
        {
            new UpdateUserDataParams {Value = NewFirstName, UpdateType = UpdateType.Value},
            new UpdateUserDataParams {Type = LastNameDataType, UpdateType = UpdateType.Type},
            new UpdateUserDataParams {LevelOfAssurance = LevelOfAssuranceEnum.High, UpdateType = UpdateType.LevelOfAssurance},
        };

        public UserDataVersioningProxyTest()
        {
            var databaseFactory = new MemoryDatabaseFactory();
            var mappings = new AuthorizationMappingProvider().GetMappings();
            m_sessionManager = new MockDbFactory(databaseFactory, mappings).CreateSessionManager(true);

            var mockFactory = new MockRepository(MockBehavior.Loose) {CallBase = true};

            m_userDataRepository = mockFactory.Create<UserDataRepository>(m_sessionManager).Object;
            var userDataComparerMock = mockFactory.Create<UserDataEqualityComparer>();
            var userDataStructureConvertorMock = mockFactory.Create<UserDataStructureConvertor>();

            m_dateTimeProviderMock = mockFactory.Create<IDateTimeProvider>();
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            m_dataVersioningProxy = new UserDataVersioningProxy(m_userDataRepository, userDataComparerMock.Object,
                m_dateTimeProviderMock.Object, userDataStructureConvertorMock.Object);

            m_testUser = CreateTestUser(Username);
            m_lowLevelOfAssurance = CreateTestLoa(MediumLoa);
            m_firstNameDataType = CreateTestDataType(FirstNameDataType);
            m_lastNameDataType = CreateTestDataType(LastNameDataType);
            m_userDataSource = CreateTestDataSource(DataSourceEnum.User);
        }

        /// <summary>
        /// Check if create user data stores correct number of user data
        /// </summary>
        [Fact]
        public void CreateUserData()
        {
            var userData = CreateUserDataData;

            m_dataVersioningProxy.CreateUserData(userData);

            var userDataRowCount = m_sessionManager.OpenSession().QueryOver<UserDataEntity>().RowCount();

            Assert.Equal(userData.Count, userDataRowCount);
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns correct version of user data
        /// </summary>
        [Fact]
        public void FindLatestUserDataByUserId_Pass()
        {
            var actualUserData = GenerateCurrentUserDataForTestUser();
            var outdatedUserData = GenerateOutdatedUserDataForTestUser();

            SaveNewUserData(actualUserData.Concat(outdatedUserData));

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithTreeStructure(m_testUser.Id);

            Assert.Equal(actualUserData.Count, loadedUserData.Count);
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns only UserData without parent in top level
        /// </summary>
        [Fact]
        public void FindLatestUserDataByUserId_TreeWithoutChildrenInRoot()
        {
            CreateTestTreeStructure();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithTreeStructure(m_testUser.Id);

            foreach (var userDataEntity in loadedUserData)
            {
                Assert.Null(userDataEntity.ParentUserData);
            }
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns only actual children UserData
        /// </summary>
        [Fact]
        public void FindLatestUserDataByUserId_TreeWithLatestChildrenOnly()
        {
            CreateTestTreeStructure();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithTreeStructure(m_testUser.Id);

            foreach (var userDataEntity in loadedUserData)
            {
                foreach (var childUserData in userDataEntity.ChildrenUserData)
                {
                    Assert.Null(childUserData.ActiveTo);
                }
            }
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns empty list when specifying id of user that has no user data
        /// </summary>
        [Fact]
        public void FindLatestUserDataByUserId_EmptyList()
        {
            CreateTestUserData();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithTreeStructure(m_testUser.Id + 1);

            Assert.Empty(loadedUserData);
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns correct version of user data with correct values
        /// </summary>
        [Fact]
        public void FindLatestUserDataByParameters()
        {
            CreateTestUserData();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa);
            var sourceUserCurrentUserData = loadedUserData.Single(x => x.DataSource.Equals(m_userDataSource));

            Assert.Null(sourceUserCurrentUserData.ActiveTo);
            Assert.Equal(FirstName, sourceUserCurrentUserData.Value);
            Assert.Equal(FirstNameDataType, sourceUserCurrentUserData.UserDataType.DataTypeValue);
            Assert.True(sourceUserCurrentUserData.LevelOfAssurance.Level > (int) LowLoa);
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns empty list for non existing value of specified type
        /// </summary>
        [Theory]
        [MemberData(nameof(GetUserDataListDataFail))]
        public void FindLatestUserDataByParameters_Fail(GetUserDataParams data)
        {
            CreateTestUserData();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(data.Value, data.Type, data.LevelOfAssurance);

            Assert.Empty(loadedUserData);
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns only UserData without parent in top level
        /// </summary>
        [Fact]
        public void FindLatestUserDataByParameters_TreeWithoutChildrenInRoot()
        {
            CreateTestTreeStructure();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, MediumLoa);

            foreach (var userDataEntity in loadedUserData)
            {
                Assert.Null(userDataEntity.ParentUserData);
            }
        }

        /// <summary>
        /// Check if FindUserDataWithTreeStructure returns only actual children UserData
        /// </summary>
        [Fact]
        public void FindLatestUserDataByParameters_TreeWithLatestChildrenOnly()
        {
            CreateTestTreeStructure();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, MediumLoa);

            foreach (var userDataEntity in loadedUserData)
            {
                foreach (var childUserData in userDataEntity.ChildrenUserData)
                {
                    Assert.Null(childUserData.ActiveTo);
                }
            }
        }

        /// <summary>
        /// Checks if UpdateUserData does not update user data when they are the same as the one in database
        /// </summary>
        [Fact]
        public void UpdateWithoutModifyingValue()
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestUserData();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa);
            var sourceUserCurrentUserData = loadedUserData.Single(x => x.DataSource.Equals(m_userDataSource));

            m_dataVersioningProxy.UpdateUserData(new[] {sourceUserCurrentUserData});

            var latestLoadedUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa, now);
            var latestSourceUserCurrentUserData = latestLoadedUserData.Single(x => x.DataSource.Equals(m_userDataSource));

            Assert.Equal(sourceUserCurrentUserData, latestSourceUserCurrentUserData); // default comparer using only ID for comparision
            Assert.Null(latestSourceUserCurrentUserData.ActiveTo);
        }

        /// <summary>
        /// Checks if UpdateUserData sets ActiveTo property of updated version and does not update other columns
        /// </summary>
        [Theory]
        [MemberData(nameof(UpdateUserDataData))]
        public void UpdateAfterModifyingValue_CheckOldVersion(UpdateUserDataParams data)
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestUserData();

            var loadedUserData = m_dataVersioningProxy.FindUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa);
            var sourceUserCurrentUserData = loadedUserData.Single(x => x.DataSource.Equals(m_userDataSource));
            SetUserData(sourceUserCurrentUserData, data);

            m_dataVersioningProxy.UpdateUserData(new[] {sourceUserCurrentUserData});

            var oldVersion = m_userDataRepository.FindById<UserDataEntity>(sourceUserCurrentUserData.Id);

            Assert.NotNull(oldVersion.ActiveTo);
            Assert.Equal(now, oldVersion.ActiveTo);
            Assert.Equal(FirstName, oldVersion.Value);
            Assert.Equal(FirstNameDataType, oldVersion.UserDataType.DataTypeValue);
            Assert.Equal(MediumLoa, oldVersion.LevelOfAssurance.Name);
        }

        /// <summary>
        /// Checks if UpdateUserData creates new version of UserData with updated properties and null ActiveTo
        /// </summary>
        [Theory]
        [MemberData(nameof(UpdateUserDataData))]
        public void UpdateAfterModifyingValue_CheckNewVersion(UpdateUserDataParams data)
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestUserData();

            var currentUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa, now);
            var sourceUserCurrentUserData = currentUserData.Single(x => x.DataSource.Equals(m_userDataSource));
            SetUserData(sourceUserCurrentUserData, data);

            m_dataVersioningProxy.UpdateUserData(new[] {sourceUserCurrentUserData});

            m_sessionManager.OpenSession()
                .Flush(); //Flush has to be here, otherwise NHibernate returns old version with specified ValidTo even though WHERE clause in FindActualVersionOfUserDataWithLoaGreaterThan is set to ValidTo == null
            var newUserDataVersion = m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(sourceUserCurrentUserData.Value,
                sourceUserCurrentUserData.UserDataType.DataTypeValue, LowLoa, now);
            var sourceUserNewUserData = newUserDataVersion.Single(x => x.DataSource.Equals(m_userDataSource));

            CheckUpdatedUserData(sourceUserNewUserData, data);
            Assert.Null(sourceUserNewUserData.ActiveTo);
            Assert.Equal(now, sourceUserNewUserData.ActiveFrom);

            Assert.Equal(sourceUserCurrentUserData.DataSource, sourceUserNewUserData.DataSource);
            Assert.Equal(sourceUserCurrentUserData.ParentUserData, sourceUserNewUserData.ParentUserData);
            Assert.Equal(sourceUserCurrentUserData.User, sourceUserNewUserData.User);
            Assert.Equal(sourceUserCurrentUserData.VerifiedBy, sourceUserNewUserData.VerifiedBy);
            Assert.NotEqual(sourceUserCurrentUserData.Id, sourceUserNewUserData.Id);
        }

        /// <summary>
        /// Checks if parent of UserData is not modified after new version of child is created
        /// </summary>
        [Fact]
        public void UpdateChild_CheckParent()
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestTreeStructure();

            var parentOld = m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(null, NameDataType, LowLoa, now).First();
            parentOld.ChildrenUserData.First(x => x.UserDataType.DataTypeValue == FirstNameDataType).Value = NewFirstName;

            m_dataVersioningProxy.UpdateUserData(new[] {parentOld});

            var parentNew = m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(null, NameDataType, LowLoa, now).First();

            Assert.Equal(parentOld.Id, parentNew.Id);
            Assert.Equal(parentOld.ChildrenUserData.Count + 1, parentNew.ChildrenUserData.Count);

            Assert.Equal(parentOld.ActiveTo, parentNew.ActiveTo);
            Assert.Equal(parentOld.ActiveFrom, parentNew.ActiveFrom);
            Assert.Equal(parentOld.DataSource, parentNew.DataSource);
            Assert.Equal(parentOld.LevelOfAssurance, parentNew.LevelOfAssurance);
            Assert.Equal(parentOld.ParentUserData, parentNew.ParentUserData);
            Assert.Equal(parentOld.User, parentNew.User);
            Assert.Equal(parentOld.UserDataType, parentNew.UserDataType);
            Assert.Equal(parentOld.VerifiedBy, parentNew.VerifiedBy);
        }

        /// <summary>
        /// Checks if child UserData are correctly linked to new version of parent
        /// </summary>
        [Fact]
        public void UpdateParent_CheckChild()
        {
            var updatedParentValue = "UpdatedValue";

            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestTreeStructure();

            var parentOld = m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(null, NameDataType, LowLoa, now).First();
            parentOld.Value = updatedParentValue;

            m_dataVersioningProxy.UpdateUserData(new[] {parentOld});

            var parentNew = m_userDataRepository
                .FindCurrentVersionOfUserDataWithLoaGreaterThan(updatedParentValue, NameDataType, LowLoa, now).First();

            var childFirstName = m_userDataRepository
                .FindCurrentVersionOfUserDataWithLoaGreaterThan(FirstName, FirstNameDataType, LowLoa, now).First();
            var childLastName = m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(LastName, LastNameDataType, LowLoa, now)
                .First();

            Assert.Equal(childFirstName.ParentUserData, parentNew);
            Assert.Equal(childLastName.ParentUserData, parentNew);
        }

        /// <summary>
        /// Checks if UserData is correctly updated when ActiveTo date is shifted to the future
        /// </summary>
        [Fact]
        public void UpdateActiveToOfUserData_CheckPositiveShift()
        {
            var idDataType = CreateTestDataType(TimeLimitedDataType);

            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            var originalValidFrom = now;
            var originalValidTo = now + TimeSpan.FromDays(10);
            const string idValue = "12345678";

            var outdatedId = GenerateOriginalIdUserData(originalValidFrom, originalValidTo, idDataType, idValue);
            var originalEntityId = SaveNewUserData(outdatedId);
            m_sessionManager.OpenSession().Flush(); // force NHibernate to write out data

            var originalCheckTime = now + TimeSpan.FromTicks(1); // new version of user data is valid only after the moment it was written

            var outdatedUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(idValue, idDataType.DataTypeValue, LowLoa,
                    originalCheckTime);
            var sourceUserOutdatedUserData = outdatedUserData.SingleOrDefault(x => x.DataSource.Equals(m_userDataSource));

            Assert.NotNull(sourceUserOutdatedUserData);
            Assert.Equal(idValue, sourceUserOutdatedUserData.Value);
            Assert.Equal(originalValidTo, sourceUserOutdatedUserData.ActiveTo);
            Assert.Equal(originalValidFrom, sourceUserOutdatedUserData.ActiveFrom);

            var newValidity = now + TimeSpan.FromDays(30);

            sourceUserOutdatedUserData.ActiveTo = newValidity;

            var newWriteTime = originalCheckTime;

            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(newWriteTime);

            m_dataVersioningProxy.UpdateUserData(new[] {sourceUserOutdatedUserData});
            m_sessionManager.OpenSession().Flush(); // force NHibernate to write out data

            var newCheckTime = newWriteTime + TimeSpan.FromTicks(1);
            var newUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(idValue, idDataType.DataTypeValue, LowLoa,
                    newCheckTime);
            var sourceUserNewUserData = newUserData.SingleOrDefault(x => x.DataSource.Equals(m_userDataSource));

            Assert.NotNull(sourceUserNewUserData);
            Assert.Equal(idValue, sourceUserNewUserData.Value);
            Assert.Equal(newValidity, sourceUserNewUserData.ActiveTo);
            Assert.Equal(newWriteTime, sourceUserNewUserData.ActiveFrom);

            var expiredEntity = m_sessionManager.OpenSession().Get<UserDataEntity>(originalEntityId);
            Assert.Equal(idValue, expiredEntity.Value);
            Assert.Equal(newWriteTime, expiredEntity.ActiveTo); // verify that original ActiveTo was updated to new one
            Assert.Equal(originalValidFrom, expiredEntity.ActiveFrom);
        }

        /// <summary>
        /// Checks that expired UserData is not modified and there is no continuity from expired entry to current one
        /// </summary>
        [Fact]
        public void UpdateActiveToOfExpiredUserData_CheckContinuity()
        {
            var idDataType = CreateTestDataType(TimeLimitedDataType);

            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            var expiredValidFrom = now + TimeSpan.FromDays(-20);
            var expiredValidTo = now + TimeSpan.FromDays(-10);
            const string idValue = "12345678";

            var expiredId = GenerateOriginalIdUserData(expiredValidFrom, expiredValidTo, idDataType, idValue);
            var expiredEntityId = SaveNewUserData(expiredId);
            m_sessionManager.OpenSession().Flush(); // force NHibernate to write out data

            var originalCheckTime = now;

            var outdatedUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(idValue, idDataType.DataTypeValue, LowLoa,
                    originalCheckTime);
            var sourceUserOutdatedUserData = outdatedUserData.SingleOrDefault(x => x.DataSource.Equals(m_userDataSource));

            Assert.Null(sourceUserOutdatedUserData); // check it is not possible to get id of expired UserData

            //var newValidFrom = now;
            var newValidTo = now + TimeSpan.FromDays(30);

            var newWriteTime = originalCheckTime;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(newWriteTime);

            var expiredEntity = m_sessionManager.OpenSession().QueryOver<UserDataEntity>().Where(x => x.Id == expiredEntityId)
                .SingleOrDefault(); // attempt to update userdata bypassing FindCurrentVersionOfUserData* methods
            //expiredEntity.ActiveFrom = newValidFrom; // ActiveFrom is ignored for saving
            expiredEntity.ActiveTo = newValidTo;
            m_dataVersioningProxy.UpdateUserData(new[] {expiredEntity});
            m_sessionManager.OpenSession().Flush(); // force NHibernate to write out data
            
            var newCheckTime =
                newWriteTime + TimeSpan.FromTicks(1); // new version of user data is valid only after the moment it was written
            var newUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(idValue, idDataType.DataTypeValue, LowLoa,
                    newCheckTime);
            var sourceUserNewUserData = newUserData.SingleOrDefault(x => x.DataSource.Equals(m_userDataSource));

            Assert.NotNull(sourceUserNewUserData);
            Assert.Equal(idValue, sourceUserNewUserData.Value);
            Assert.Equal(newValidTo, sourceUserNewUserData.ActiveTo);
            Assert.Equal(newWriteTime, sourceUserNewUserData.ActiveFrom);

            var expiredEntityAfterUpdate = m_sessionManager.OpenSession().QueryOver<UserDataEntity>().Where(x => x.Id == expiredEntityId)
                .SingleOrDefault(); // attempt to update userdata bypassing FindCurrentVersionOfUserData* methods
            Assert.Equal(idValue, expiredEntityAfterUpdate.Value);
            Assert.Equal(expiredValidTo, expiredEntityAfterUpdate.ActiveTo); // verify that original version still has original time periods
            Assert.Equal(expiredValidFrom, expiredEntityAfterUpdate.ActiveFrom);
        }

        /// <summary>
        /// Checks if UserData is throws exception if trying to create or update userdata with already expired data
        /// </summary>
        [Fact]
        public void UpdateActiveToToExpired_CheckThrowsException()
        {
            var idDataType = CreateTestDataType(TimeLimitedDataType);

            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            var originalValidFrom = now;
            var originalValidTo = now + TimeSpan.FromDays(10);
            const string idValue = "12345678";

            var outdatedId = GenerateOriginalIdUserData(originalValidFrom, originalValidTo, idDataType, idValue);
            SaveNewUserData(new[] {outdatedId});
            m_sessionManager.OpenSession().Flush(); // force NHibernate to write out data

            var originalCheckTime = now + TimeSpan.FromTicks(1); // new version of user data is valid only after the moment it was written

            var outdatedUserData =
                m_userDataRepository.FindCurrentVersionOfUserDataWithLoaGreaterThan(idValue, idDataType.DataTypeValue, LowLoa,
                    originalCheckTime);
            var sourceUserOutdatedUserData = outdatedUserData.SingleOrDefault(x => x.DataSource.Equals(m_userDataSource));

            Assert.NotNull(sourceUserOutdatedUserData);

            var newValidity = now + TimeSpan.FromMinutes(-1);

            sourceUserOutdatedUserData.ActiveTo = newValidity;

            var newWriteTime = originalCheckTime;

            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(newWriteTime);

            Assert.Throws<InvalidOperationException>(() => m_dataVersioningProxy.UpdateUserData(new[] { sourceUserOutdatedUserData }));
        }

        private void SetUserData(UserDataEntity loadedUserData, UpdateUserDataParams data)
        {
            switch (data.UpdateType)
            {
                case UpdateType.Value:
                    loadedUserData.Value = data.Value;
                    break;
                case UpdateType.Type:
                    loadedUserData.UserDataType = CreateTestDataType(data.Type);
                    break;
                case UpdateType.LevelOfAssurance:
                    loadedUserData.LevelOfAssurance = CreateTestLoa(data.LevelOfAssurance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckUpdatedUserData(UserDataEntity updatedUserData, UpdateUserDataParams data)
        {
            switch (data.UpdateType)
            {
                case UpdateType.Value:
                    Assert.Equal(data.Value, updatedUserData.Value);
                    break;
                case UpdateType.Type:
                    Assert.Equal(data.Type, updatedUserData.UserDataType.DataTypeValue);
                    break;
                case UpdateType.LevelOfAssurance:
                    Assert.Equal(data.LevelOfAssurance, updatedUserData.LevelOfAssurance.Name);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Methods that creates and saves test user, loa, data type and data source.

        private UserEntity CreateTestUser(string username)
        {
            //Only username can be used in these tests, so other not nullable properties can be random/hardcoded
            var user = new UserEntity
            {
                Username = username, PasswordHash = "132456879", TwoFactorEnabled = true, LockoutEnabled = true, AccessFailedCount = 0,
                LastChange = DateTime.UtcNow,
            };
            var id = (int) m_sessionManager.OpenSession().Save(user);

            var loadedUser = m_sessionManager.OpenSession().Get<UserEntity>(id);

            return loadedUser;
        }

        private LevelOfAssuranceEntity CreateTestLoa(LevelOfAssuranceEnum levelOfAssurance)
        {
            var loa = new LevelOfAssuranceEntity() {Level = (int) levelOfAssurance, Name = levelOfAssurance};
            var id = (int) m_sessionManager.OpenSession().Save(loa);

            var loadedLoa = m_sessionManager.OpenSession().Get<LevelOfAssuranceEntity>(id);

            return loadedLoa;
        }

        private UserDataTypeEntity CreateTestDataType(string dataTypeValue)
        {
            var dataType = new UserDataTypeEntity {DataTypeValue = dataTypeValue};
            var id = (int) m_sessionManager.OpenSession().Save(dataType);

            var loadedDataType = m_sessionManager.OpenSession().Get<UserDataTypeEntity>(id);

            return loadedDataType;
        }

        private DataSourceEntity CreateTestDataSource(DataSourceEnum dataSourceEnum)
        {
            var dataSource = new DataSourceEntity {DataSource = dataSourceEnum};
            var id = (int) m_sessionManager.OpenSession().Save(dataSource);

            var loadedDataSource = m_sessionManager.OpenSession().Get<DataSourceEntity>(id);

            return loadedDataSource;
        }

        #endregion


        private int SaveNewUserData(UserDataEntity userData)
        {
            m_userDataRepository.Create(userData);
            return userData.Id;
        }

        private void SaveNewUserData(IEnumerable<UserDataEntity> userDataList)
        {
            m_userDataRepository.CreateAll(userDataList);
        }

        private void CreateTestUserData()
        {
            var currentUserData = GenerateCurrentUserDataForTestUser();
            var outdatedUserData = GenerateOutdatedUserDataForTestUser();

            SaveNewUserData(currentUserData.Concat(outdatedUserData));
        }

        private void CreateTestTreeStructure()
        {
            var nameDataType = CreateTestDataType(NameDataType);

            var parent = GenerateUserDataEntity(m_activeFrom, nameDataType, m_testUser, m_userDataSource, m_lowLevelOfAssurance);

            var id = (int) m_sessionManager.OpenSession().Save(parent);

            var loadedParent = m_sessionManager.OpenSession().Get<UserDataEntity>(id);

            var childFirstName = GenerateUserDataEntity(m_activeFrom, m_firstNameDataType, m_testUser, m_userDataSource,
                m_lowLevelOfAssurance, FirstName);
            childFirstName.ParentUserData = loadedParent;

            var childLastName = GenerateUserDataEntity(m_activeFrom, m_lastNameDataType, m_testUser, m_userDataSource,
                m_lowLevelOfAssurance, LastName);
            childLastName.ParentUserData = loadedParent;

            var oldChildFirstName = GenerateUserDataEntity(m_activeFrom, m_firstNameDataType, m_testUser, m_userDataSource,
                m_lowLevelOfAssurance, OldFirstName, m_activeTo);
            oldChildFirstName.ParentUserData = loadedParent;

            loadedParent.ChildrenUserData = new List<UserDataEntity>
            {
                childFirstName,
                childLastName,
                oldChildFirstName
            };

            m_sessionManager.OpenSession().Save(childFirstName);
            m_sessionManager.OpenSession().Save(childLastName);
            m_sessionManager.OpenSession().Save(oldChildFirstName);
            m_sessionManager.OpenSession().Save(loadedParent);
        }

        #region Methods for generating user data

        private UserDataEntity GenerateUserDataEntity(
            DateTime activeFrom,
            UserDataTypeEntity dataType,
            UserEntity user,
            DataSourceEntity dataSource,
            LevelOfAssuranceEntity loa,
            string value = null,
            DateTime? activeTo = null)
        {
            return new UserDataEntity
            {
                ActiveFrom = activeFrom,
                ActiveTo = activeTo,
                DataSource = dataSource,
                LevelOfAssurance = loa,
                Value = value,
                UserDataType = dataType,
                User = user,
            };
        }

        private UserDataEntity GenerateOriginalIdUserData(DateTime now, DateTime activeTo, UserDataTypeEntity idDataType,
            string value)
        {
            return GenerateUserDataEntity(now, idDataType, m_testUser, m_userDataSource, m_lowLevelOfAssurance, value, activeTo);
        }

        private IList<UserDataEntity> GenerateCurrentUserDataForTestUser()
        {
            return new List<UserDataEntity>
            {
                GenerateUserDataEntity(m_activeFrom, m_firstNameDataType, m_testUser, m_userDataSource, m_lowLevelOfAssurance, FirstName),
                GenerateUserDataEntity(m_activeFrom, m_lastNameDataType, m_testUser, m_userDataSource, m_lowLevelOfAssurance, LastName),
            };
        }

        private IList<UserDataEntity> GenerateOutdatedUserDataForTestUser()
        {
            return new List<UserDataEntity>
            {
                GenerateUserDataEntity(m_activeFrom, m_firstNameDataType, m_testUser, m_userDataSource, m_lowLevelOfAssurance, OldFirstName,
                    m_activeTo),
            };
        }

        #endregion

        public class GetUserDataParams
        {
            public string Value { get; set; }
            public string Type { get; set; }
            public LevelOfAssuranceEnum LevelOfAssurance { get; set; }
        }

        public class UpdateUserDataParams
        {
            public string Value { get; set; }
            public string Type { get; set; }
            public LevelOfAssuranceEnum LevelOfAssurance { get; set; }
            public UpdateType UpdateType { get; set; }
        }

        public enum UpdateType
        {
            Value,
            Type,
            LevelOfAssurance,
        }
    }
}