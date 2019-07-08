using System.Collections.Generic;
using AutoMapper;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class DictionaryUserDataBasicInfoResolver<T> : IValueResolver<UserBasicInfoModel, T, Dictionary<string, string>>
    {
        public Dictionary<string, string> Resolve(UserBasicInfoModel source, T destination, Dictionary<string, string> destMember, ResolutionContext context)
        {
            return GetUserDataDictionary(source.UserData);
        }

        private Dictionary<string, string> GetUserDataDictionary(IList<UserDataBasicInfoModel> userDataList)
        {
            if (userDataList == null)
            {
                return null;
            }

            var dict = new Dictionary<string, string>();

            foreach (var userData in userDataList)
            {
                dict.Add(userData.UserDataType, userData.Value);

                if (userData.ChildrenUserData != null)
                {
                    var childDict = GetUserDataDictionary(userData.ChildrenUserData);
                    foreach (var (key, value) in childDict)
                    {
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add(key, value);
                        }
                    }
                }
            }

            return dict;
        }
    }
}