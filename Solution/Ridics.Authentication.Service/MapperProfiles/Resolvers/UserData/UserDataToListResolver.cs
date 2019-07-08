using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.UserData;
using Ridics.Authentication.Service.Exceptions;
using Ridics.Core.Shared.Types;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class UserDataToListResolver<T> : IValueResolver<T, UserModel, IList<UserDataModel>> where T : IConvertibleToUserData
    {
        private readonly IUserDataTypeManager m_userDataTypeManager;
        private List<UserDataTypeModel> m_userDataTypes;

        public UserDataToListResolver(IUserDataTypeManager userDataTypeManager)
        {
            m_userDataTypeManager = userDataTypeManager;
        }

        private IList<UserDataTypeModel> GetAllUserDataTypes
        {
            get
            {
                if (m_userDataTypes != null)
                {
                    return m_userDataTypes;
                }

                var userDataTypesResult = m_userDataTypeManager.GetAllUserDataTypes();

                if (userDataTypesResult.HasError)
                {
                    throw new UserDataException("Failed to get user data types"); //TODO catch exception during mapping
                }

                m_userDataTypes = userDataTypesResult.Result;

                return m_userDataTypes;
            }
        }

        protected virtual List<UserDataPropertyMapping> GetMappings(T source)
        {
            var mappings = new List<UserDataPropertyMapping>
            {
                new UserDataPropertyMapping
                {
                    PropertyValue = source.Title.ToString("G"), DataTypeValue = CustomUserDataTypes.Title
                },
                new UserDataPropertyMapping {PropertyValue = source.FullName, DataTypeValue = CustomUserDataTypes.FullName},
                new UserDataPropertyMapping {PropertyValue = source.Prefix, DataTypeValue = CustomUserDataTypes.Prefix},
                new UserDataPropertyMapping {PropertyValue = source.SecondName, DataTypeValue = CustomUserDataTypes.SecondName},
                new UserDataPropertyMapping {PropertyValue = source.Suffix, DataTypeValue = CustomUserDataTypes.Suffix},
                new UserDataPropertyMapping {PropertyValue = source.MasterUserId.ToString(), DataTypeValue = UserDataTypes.MasterUserId},
                new UserDataPropertyMapping {PropertyValue = source.FirstName, DataTypeValue = UserDataTypes.FirstName},
                new UserDataPropertyMapping {PropertyValue = source.LastName, DataTypeValue = UserDataTypes.LastName}
            };

            return mappings;
        }

        public IList<UserDataModel> Resolve(T source, UserModel destination, IList<UserDataModel> destMember,
            ResolutionContext context)
        {
            try
            {
                var userDataList = new List<UserDataModel>();
                var mappings = GetMappings(source);

                foreach (var userDataPropertyMapping in mappings)
                {
                    AddToList(userDataPropertyMapping, userDataList, destination);
                }

                return userDataList;
            }
            catch (InvalidOperationException)
            {
                throw new UserDataException("Error during user data selection"); //TODO catch exception during mapping
            }
        }

        private void AddToList(UserDataPropertyMapping propertyMapping, List<UserDataModel> userDataList, UserModel user)
        {
            if (propertyMapping.ChildrenMappings != null &&
                propertyMapping.ChildrenMappings.All(x => !string.IsNullOrEmpty(x.PropertyValue)) ||
                !string.IsNullOrEmpty(propertyMapping.PropertyValue))
            {
                userDataList.Add(CreateUserDataModel(user, propertyMapping));
            }
        }

        private UserDataModel CreateUserDataModel(UserModel user, UserDataPropertyMapping propertyMapping)
        {
            var userDataModel = new UserDataModel
            {
                User = user,
                UserDataType = GetAllUserDataTypes.Single(x => x.DataTypeValue == propertyMapping.DataTypeValue),
                Value = propertyMapping.PropertyValue,
                ActiveTo = propertyMapping.ValueValidTo,
            };

            if (propertyMapping.ChildrenMappings != null)
            {
                var childrenDataModels = new List<UserDataModel>();

                foreach (var childrenMapping in propertyMapping.ChildrenMappings)
                {
                    childrenDataModels.Add(CreateUserDataModel(user, childrenMapping));
                }

                userDataModel.ChildrenUserData = childrenDataModels;
            }

            return userDataModel;
        }

        protected class UserDataPropertyMapping
        {
            public string PropertyValue { get; set; }

            public string DataTypeValue { get; set; }

            public DateTime? ValueValidTo { get; set; }

            public ISet<UserDataPropertyMapping> ChildrenMappings { get; set; }
        }
    }
}