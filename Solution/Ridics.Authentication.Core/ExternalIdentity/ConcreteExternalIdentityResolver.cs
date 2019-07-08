using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.ExternalIdentity
{
    public class ConcreteExternalIdentityResolver : IConcreteExternalIdentityResolver
    {
        private readonly string m_dataTypeValue;
        private readonly IList<string> m_externalIdentitiesNames;
        private readonly ILogger<ConcreteExternalIdentityResolver> m_logger;

        public ConcreteExternalIdentityResolver(string dataTypeValue, IList<string> externalIdentitiesNames,
            ILogger<ConcreteExternalIdentityResolver> logger)
        {
            m_dataTypeValue = dataTypeValue;
            m_externalIdentitiesNames = externalIdentitiesNames;
            m_logger = logger;
        }

        public IList<UserExternalIdentityModel> Resolve(IList<UserDataModel> userDataList, IList<ExternalIdentityModel> externalIdentities)
        {
            var userExternalIdentities = new List<UserExternalIdentityModel>();

            foreach (var externalIdentityName in m_externalIdentitiesNames)
            {
                var externalIdentity = externalIdentities.SingleOrDefault(x => x.Name == externalIdentityName);

                var externalIdentityUserData = GetFlatUserDataList(userDataList).SingleOrDefault(x => x.UserDataType.DataTypeValue == m_dataTypeValue);

                var userExternalIdentity = new UserExternalIdentityModel
                {
                    ExternalIdentity = externalIdentityUserData?.Value,
                    ExternalIdentityType = externalIdentity
                };

                if (userExternalIdentity.ExternalIdentityType == null)
                {
                    if (m_logger.IsEnabled(LogLevel.Warning))
                    {
                        m_logger.LogWarning("Can not resolve external identity with name {0}.", externalIdentityName);
                    }

                    continue; //Do not add to collection if external id not resolved properly
                }

                if (userExternalIdentity.ExternalIdentity == null)
                {
                    if (m_logger.IsEnabled(LogLevel.Warning))
                    {
                        m_logger.LogWarning("Can not resolve external identity data with data type {0}.", m_dataTypeValue);
                    }

                    continue; //Do not add to collection if external id not resolved properly
                }

                userExternalIdentities.Add(userExternalIdentity);
            }

            return userExternalIdentities;
        }

        private IList<UserDataModel> GetFlatUserDataList(IList<UserDataModel> userData)
        {
            if (userData == null)
            {
                return null;
            }

            var userDataList = new List<UserDataModel>();

            foreach (var userDataModel in userData)
            {
                userDataList.Add(userDataModel);

                var children = GetFlatUserDataList(userDataModel.ChildrenUserData);

                if (children != null)
                {
                    userDataList.AddRange(children);
                }
            }

            var hashSet = new HashSet<UserDataModel>(); //Prevent duplicate types

            foreach (var userDataEntity in userDataList)
            {
                hashSet.Add(userDataEntity);
            }

            return hashSet.ToList();
        }
    }
}