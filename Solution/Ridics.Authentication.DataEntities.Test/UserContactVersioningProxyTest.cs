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
using Ridics.Core.Shared.Providers;
using Ridics.Core.Test.Shared.Database;
using Xunit;

namespace Ridics.Authentication.DataEntities.Test
{
    public class UserContactVersioningProxyTest
    {
        private const string PhoneValue = "+420123456789";
        private const string OldPhoneValue = "+420987654321";
        private const string EmailValue = "tester@testerovic.com";
        private const string OldEmailValue = "testerovic@tester.com";
        private const string NewEmailValue = "example@example.com";
        private const string UsernameValue = "TestUsername";
        private const string ConfirmCodeValue = "1234";
        private const LevelOfAssuranceEnum MediumLoa = LevelOfAssuranceEnum.Substantial;
        private const LevelOfAssuranceEnum LowLoa = LevelOfAssuranceEnum.Low;

        private static readonly DateTime m_activeFrom = new DateTime(2019, 1, 1, 0, 0, 0);
        private static readonly DateTime? m_activeTo = new DateTime(2019, 2, 1, 0, 0, 0);
        private static readonly DateTime m_confirmCodeChangeTime = new DateTime(2019, 2, 1, 0, 0, 0);


        private readonly UserContactVersioningProxy m_contactVersioningProxy;
        private readonly ISessionManager m_sessionManager;

        private readonly UserContactRepository m_userContactRepository;

        private readonly Mock<IDateTimeProvider> m_dateTimeProviderMock;

        private readonly UserEntity m_testUser;
        private readonly LevelOfAssuranceEntity m_lowLevelOfAssurance;
        private readonly DataSourceEntity m_userDataSource;

        public static readonly List<UserContactEntity> CreateUserContactData = new List<UserContactEntity>
        {
            new UserContactEntity{ActiveFrom = DateTime.UtcNow.AddHours(-1), ActiveTo = null, DataSource = null, LevelOfAssurance = null, User = null, Value = "test@example.com", Type = ContactTypeEnum.Email, ConfirmCode = null,ConfirmCodeChangeTime = null},
            new UserContactEntity{ActiveFrom = DateTime.UtcNow.AddDays(-1), ActiveTo = DateTime.UtcNow.AddHours(-1), DataSource = null, LevelOfAssurance = null, User = null, Value = "+420123456789", Type = ContactTypeEnum.Phone, ConfirmCode = null,ConfirmCodeChangeTime = null},
            new UserContactEntity{ActiveFrom = DateTime.UtcNow, ActiveTo = null, DataSource = null, LevelOfAssurance = null, User = null, Value = "testerovic@example.com", Type = ContactTypeEnum.Email, ConfirmCode = "123456", ConfirmCodeChangeTime = DateTime.UtcNow},
            new UserContactEntity{ActiveFrom = DateTime.UtcNow, ActiveTo = null, DataSource = null, LevelOfAssurance = null, User = null, Value = "+420987654321", Type = ContactTypeEnum.Phone, ConfirmCode = null, ConfirmCodeChangeTime = DateTime.UtcNow},
        };

        public static readonly TheoryData<GetUserContactParams> GetContactsDataFail = new TheoryData<GetUserContactParams>
        {
            new GetUserContactParams {Value = "incorrect@example.com", Type = ContactTypeEnum.Email, LevelOfAssurance = LevelOfAssuranceEnum.Low},
            new GetUserContactParams {Value = "test@example.com", Type = ContactTypeEnum.Phone, LevelOfAssurance = LevelOfAssuranceEnum.Low},
            new GetUserContactParams {Value = "test@example.com", Type = ContactTypeEnum.Email, LevelOfAssurance = LevelOfAssuranceEnum.High},
        };

        public static readonly TheoryData<UpdateUserContactParams> UpdateUserContactData = new TheoryData<UpdateUserContactParams>
        {
            new UpdateUserContactParams {Value = NewEmailValue, UpdateType = UpdateType.Value},
            new UpdateUserContactParams {LevelOfAssurance = LevelOfAssuranceEnum.High, UpdateType = UpdateType.LevelOfAssurance},
            new UpdateUserContactParams {ConfirmCode = ConfirmCodeValue, UpdateType = UpdateType.ConfirmCode},
            new UpdateUserContactParams {ConfirmCodeChangeTime = m_confirmCodeChangeTime, UpdateType = UpdateType.ConfirmCodeChangeTime},
        };

        public UserContactVersioningProxyTest()
        {
            var databaseFactory = new MemoryDatabaseFactory();
            var mappings = new AuthorizationMappingProvider().GetMappings();

            m_sessionManager = new MockDbFactory(databaseFactory, mappings).CreateSessionManager(true);
            var mockFactory = new MockRepository(MockBehavior.Loose) { CallBase = true };

            m_userContactRepository = mockFactory.Create<UserContactRepository>(m_sessionManager).Object;
            var userContactComparerMock = mockFactory.Create<UserContactEqualityComparer>();

            m_dateTimeProviderMock = mockFactory.Create<IDateTimeProvider>();
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            m_contactVersioningProxy = new UserContactVersioningProxy(m_userContactRepository, userContactComparerMock.Object, m_dateTimeProviderMock.Object);

            m_testUser = CreateTestUser(UsernameValue);
            m_lowLevelOfAssurance = CreateTestLoa(MediumLoa);
            m_userDataSource = CreateTestDataSource(DataSourceEnum.User);
        }

        /// <summary>
        /// Check if create user contacts stores correct number of user contacts
        /// </summary>
        [Fact]
        public void CreateAllUserContacts()
        {
            var userContact = CreateUserContactData;

            m_contactVersioningProxy.CreateUserContacts(userContact);

            var userDataRowCount = m_sessionManager.OpenSession().QueryOver<UserContactEntity>().RowCount();

            Assert.Equal(userContact.Count, userDataRowCount);
        }

        /// <summary>
        /// Check if GetUserContact returns correct version of user contact with correct value
        /// </summary>
        [Fact]
        public void GetLatestUserContactByParameters_Pass()
        {
            var actualUserContacts = GenerateActualUserContacts();
            var outdatedUserContacts = GenerateOutdatedUserContacts();

            SaveNewUserContacts(actualUserContacts.Concat(outdatedUserContacts));

            var sourceUserContact = actualUserContacts.First(x => x.Type == ContactTypeEnum.Email);
            var loadedUserContact = m_contactVersioningProxy.GetUserContact(m_testUser.Id, ContactTypeEnum.Email);

            Assert.Equal(ContactTypeEnum.Email, loadedUserContact.Type);
            Assert.Equal(sourceUserContact.Value, loadedUserContact.Value);
            Assert.Equal(sourceUserContact.ActiveFrom, loadedUserContact.ActiveFrom);
            Assert.Equal(sourceUserContact.ActiveTo, loadedUserContact.ActiveTo);
            Assert.Equal(sourceUserContact.ConfirmCode, loadedUserContact.ConfirmCode);
            Assert.Equal(sourceUserContact.ConfirmCodeChangeTime, loadedUserContact.ConfirmCodeChangeTime);
            Assert.Null(loadedUserContact.ActiveTo);
        }

        /// <summary>
        /// Check if GetUserContact returns correct version of user contact with correct value
        /// </summary>
        [Fact]
        public void GetLatestUserContactByParameters_Fail()
        {
            CreateTestUserContacts();

            var loadedUserContact = m_contactVersioningProxy.GetUserContact(m_testUser.Id + 1, ContactTypeEnum.Email);
            
            Assert.Null(loadedUserContact);
        }

        /// <summary>
        /// Check if GetUserContactsWithLoaGreaterThan returns correct version of user contacts with correct values
        /// </summary>
        [Fact]
        public void GetLatestUserContactsByParameters_Pass()
        {
            CreateTestUserContacts();

            var loadedUserContacts = m_contactVersioningProxy.GetUserContactsWithLoaGreaterThan(EmailValue, ContactTypeEnum.Email, LowLoa);

            Assert.NotEmpty(loadedUserContacts);
            foreach (var loadedUserContact in loadedUserContacts)
            {
                Assert.Equal(ContactTypeEnum.Email, loadedUserContact.Type);
                Assert.Equal(EmailValue, loadedUserContact.Value);
                Assert.True(loadedUserContact.LevelOfAssurance.Level > (int)LowLoa);
                Assert.Null(loadedUserContact.ActiveTo);
            }
        }

        /// <summary>
        /// Check if GetUserContactsWithLoaGreaterThan returns empty list if incorrect parameters
        /// </summary>
        [Theory]
        [MemberData(nameof(GetContactsDataFail))]
        public void GetLatestUserContactsByParameters_Fail(GetUserContactParams data)
        {
            CreateTestUserContacts();

            var loadedUserContacts = m_contactVersioningProxy.GetUserContactsWithLoaGreaterThan(data.Value, data.Type, data.LevelOfAssurance);

            Assert.Empty(loadedUserContacts);
        }

        /// <summary>
        /// Check if GetUserContactsWithLoaGreaterThan returns correct version of user contacts
        /// </summary>
        [Fact]
        public void GetLatestUserContactsByUserId_Pass()
        {
            CreateTestUserContacts();

            var loadedUserContacts = m_contactVersioningProxy.GetUserContacts(m_testUser.Id);

            Assert.NotEmpty(loadedUserContacts);
            foreach (var loadedUserContact in loadedUserContacts)
            {
                Assert.Equal(m_testUser.Id, loadedUserContact.User.Id);
                Assert.Null(loadedUserContact.ActiveTo);
            }
        }

        /// <summary>
        /// Check if GetUserContactsWithLoaGreaterThan returns empty list when specifying id of user that has no contacts
        /// </summary>
        [Fact]
        public void GetLatestUserContactsByUserId_EmptyList()
        {
            CreateTestUserContacts();

            var loadedUserContacts = m_contactVersioningProxy.GetUserContacts(m_testUser.Id + 1);

            Assert.Empty(loadedUserContacts);
        }

        /// <summary>
        /// Checks if UpdateUserContacts does not update user contacts when they are the same as the ones in database
        /// </summary>
        [Fact]
        public void UpdateWithoutModifyingValue()
        {
            CreateTestUserContacts();

            var loadedUserContact = m_userContactRepository.GetActualVersionOfUserContact(m_testUser.Id, ContactTypeEnum.Email);
           
            m_contactVersioningProxy.UpdateUserContact(loadedUserContact);

            var latestLoadedUserContact = m_userContactRepository.GetActualVersionOfUserContact(m_testUser.Id, ContactTypeEnum.Email);
            
            Assert.Equal(loadedUserContact, latestLoadedUserContact); // default comparer using only ID for comparision
            Assert.Null(latestLoadedUserContact.ActiveTo);
        }

        /// <summary>
        /// Checks if UpdateUserContact sets ActiveTo property of updated version and does not update value
        /// </summary>
        [Theory]
        [MemberData(nameof(UpdateUserContactData))]
        public void UpdateAfterModifyingValue_CheckOldVersion(UpdateUserContactParams data)
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestUserContacts();

            var loadedUserContact = m_userContactRepository.GetActualVersionOfUserContact(m_testUser.Id, ContactTypeEnum.Email);
            SetUserContact(loadedUserContact, data);

            m_contactVersioningProxy.UpdateUserContact(loadedUserContact);

            var oldVersion = m_userContactRepository.FindById<UserContactEntity>(loadedUserContact.Id);

            Assert.NotNull(oldVersion.ActiveTo);
            Assert.Equal(now, oldVersion.ActiveTo);
            Assert.Equal(EmailValue, oldVersion.Value);
            Assert.Equal(ContactTypeEnum.Email, oldVersion.Type);
            Assert.Equal(MediumLoa, oldVersion.LevelOfAssurance.Name);
            Assert.Null(oldVersion.ConfirmCode);
            Assert.Null(oldVersion.ConfirmCodeChangeTime);
        }
        
        /// <summary>
        /// Checks if UpdateUserContact creates new version of UserContact with updated value and null ActiveTo
        /// </summary>
        [Theory]
        [MemberData(nameof(UpdateUserContactData))]
        public void UpdateAfterModifyingValue_CheckNewVersion(UpdateUserContactParams data)
        {
            var now = DateTime.UtcNow;
            m_dateTimeProviderMock.Setup(x => x.UtcNow).Returns(now);

            CreateTestUserContacts();

            var loadedUserContact = m_userContactRepository.GetActualVersionOfUserContact(m_testUser.Id, ContactTypeEnum.Email);
            SetUserContact(loadedUserContact, data);

            m_contactVersioningProxy.UpdateUserContact(loadedUserContact);

            m_sessionManager.OpenSession().Flush();//Flush has to be here, otherwise NHibernate returns old version with specified ValidTo even though WHERE clause in GetActualVersionOfUserContact is set to ValidTo == null
            var newVersion = m_userContactRepository.GetActualVersionOfUserContact(m_testUser.Id, ContactTypeEnum.Email);

            CheckUpdatedUserData(newVersion, data);
            Assert.Null(newVersion.ActiveTo);
            Assert.Equal(now, newVersion.ActiveFrom);

            Assert.Equal(loadedUserContact.DataSource, newVersion.DataSource);
            Assert.Equal(loadedUserContact.User, newVersion.User);
            Assert.NotEqual(loadedUserContact.Id, newVersion.Id);
        }

        private void SaveNewUserContacts(IEnumerable<UserContactEntity> userContactList)
        {
            m_userContactRepository.CreateAll(userContactList);
        }

        private void CreateTestUserContacts()
        {
            var actualUserContacts = GenerateActualUserContacts();
            var outdatedUserContacts = GenerateOutdatedUserContacts();

            SaveNewUserContacts(actualUserContacts.Concat(outdatedUserContacts));
        }

        private void SetUserContact(UserContactEntity loadedUserContact, UpdateUserContactParams data)
        {
            switch (data.UpdateType)
            {
                case UpdateType.Value:
                    loadedUserContact.Value = data.Value;
                    break;
                case UpdateType.LevelOfAssurance:
                    loadedUserContact.LevelOfAssurance = CreateTestLoa(data.LevelOfAssurance);
                    break;
                case UpdateType.ConfirmCode:
                    loadedUserContact.ConfirmCode = data.ConfirmCode;
                    break;
                case UpdateType.ConfirmCodeChangeTime:
                    loadedUserContact.ConfirmCodeChangeTime = data.ConfirmCodeChangeTime;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckUpdatedUserData(UserContactEntity updatedUserContact, UpdateUserContactParams data)
        {
            switch (data.UpdateType)
            {
                case UpdateType.Value:
                    Assert.Equal(data.Value, updatedUserContact.Value);
                    break;
                case UpdateType.LevelOfAssurance:
                    Assert.Equal(data.LevelOfAssurance, updatedUserContact.LevelOfAssurance.Name);
                    break;
                case UpdateType.ConfirmCode:
                    Assert.Equal(data.ConfirmCode, updatedUserContact.ConfirmCode);
                    break;
                case UpdateType.ConfirmCodeChangeTime:
                    Assert.Equal(data.ConfirmCodeChangeTime, updatedUserContact.ConfirmCodeChangeTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Methods that creates and saves test user, loa, data type and data source.
        private UserEntity CreateTestUser(string username)
        {
            //Only username can be used in these tests, so other not nullable properties can be random/hardcoded
            var user = new UserEntity { Username = username, PasswordHash = "132456879", TwoFactorEnabled = true, LockoutEnabled = true, AccessFailedCount = 0, LastChange = DateTime.UtcNow, };
            var id = (int)m_sessionManager.OpenSession().Save(user);

            var loadedUser = m_sessionManager.OpenSession().Get<UserEntity>(id);

            return loadedUser;
        }

        private LevelOfAssuranceEntity CreateTestLoa(LevelOfAssuranceEnum levelOfAssurance)
        {
            var loa = new LevelOfAssuranceEntity() { Level = (int)levelOfAssurance, Name = levelOfAssurance };
            var id = (int)m_sessionManager.OpenSession().Save(loa);

            var loadedLoa = m_sessionManager.OpenSession().Get<LevelOfAssuranceEntity>(id);

            return loadedLoa;
        }

        private DataSourceEntity CreateTestDataSource(DataSourceEnum dataSourceEnum)
        {
            var dataSource = new DataSourceEntity { DataSource = dataSourceEnum };
            var id = (int)m_sessionManager.OpenSession().Save(dataSource);

            var loadedDataSource = m_sessionManager.OpenSession().Get<DataSourceEntity>(id);

            return loadedDataSource;
        }
        #endregion

        #region Methods for generating user data
        private UserContactEntity GenerateUserContactEntity(
            DateTime activeFrom,
            string value,
            ContactTypeEnum type,
            UserEntity user,
            DataSourceEntity dataSource,
            LevelOfAssuranceEntity loa,
            DateTime? activeTo = null)
        {
            return new UserContactEntity
            {
                ActiveFrom = activeFrom,
                ActiveTo = activeTo,
                DataSource = dataSource,
                LevelOfAssurance = loa,
                Value = value,
                Type = type,
                User = user,
            };
        }

        private IList<UserContactEntity> GenerateActualUserContacts()
        {
            return new List<UserContactEntity>
            {
                GenerateUserContactEntity(m_activeFrom, PhoneValue, ContactTypeEnum.Phone, m_testUser, m_userDataSource, m_lowLevelOfAssurance),
                GenerateUserContactEntity(m_activeFrom, EmailValue, ContactTypeEnum.Email, m_testUser, m_userDataSource, m_lowLevelOfAssurance),
            };
        }

        private IList<UserContactEntity> GenerateOutdatedUserContacts()
        {
            return new List<UserContactEntity>
            {
                GenerateUserContactEntity(m_activeFrom, OldPhoneValue, ContactTypeEnum.Phone, m_testUser, m_userDataSource, m_lowLevelOfAssurance, m_activeTo),
                GenerateUserContactEntity(m_activeFrom, OldEmailValue, ContactTypeEnum.Email, m_testUser, m_userDataSource, m_lowLevelOfAssurance, m_activeTo),
            };
        }
        #endregion

        public class GetUserContactParams
        {
            public string Value { get; set; }
            public ContactTypeEnum Type { get; set; }
            public LevelOfAssuranceEnum LevelOfAssurance { get; set; }
        }

        public class UpdateUserContactParams
        {
            public string Value { get; set; }
            public LevelOfAssuranceEnum LevelOfAssurance { get; set; }
            public string ConfirmCode { get; set; }
            public DateTime ConfirmCodeChangeTime { get; set; }
            public UpdateType UpdateType { get; set; }
        }

        public enum UpdateType
        {
            Value,
            LevelOfAssurance,
            ConfirmCode,
            ConfirmCodeChangeTime
        }
    }
}