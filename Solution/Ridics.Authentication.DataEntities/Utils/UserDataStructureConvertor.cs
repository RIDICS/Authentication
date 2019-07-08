using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.DataEntities.Comparer;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Utils
{
    public class UserDataStructureConvertor
    {
        public IList<UserDataEntity> GetFlatUserDataList(IEnumerable<UserDataEntity> userData)
        {
            if (userData == null)
            {
                return null;
            }

            var userDataList = new List<UserDataEntity>();

            foreach (var userDataEntity in userData)
            {
                userDataList.Add(userDataEntity);

                var children = GetFlatUserDataList(userDataEntity.ChildrenUserData);

                if (children != null)
                {
                    userDataList.AddRange(children);
                }
            }

            var hashSet = new HashSet<UserDataEntity>(new UserDataTypeEqualityComparer()); //Prevent duplicate types

            foreach (var userDataEntity in userDataList)
            {
                hashSet.Add(userDataEntity);
            }

            return hashSet.ToList();
        }

        /// <summary>
        /// This method adds child UserData to its parent and returns UserData list without children UserData on same level with parent
        /// </summary>
        /// <param name="userData">UserData list with flat structure</param>
        /// <returns>UserData list with tree structure.</returns>
        public IList<UserDataEntity> GetTreeUserDataFromFlatUserDataList(IList<UserDataEntity> userData)
        {
            if (userData == null)
            {
                return null;
            }

            var childList = userData.Where(x => x.ParentUserData != null);

            foreach (var childUserData in childList)
            {
                childUserData.ParentUserData.ChildrenUserData.Add(childUserData);
            }

            return userData.Where(x => x.ParentUserData == null).ToList();
        }
    }
}