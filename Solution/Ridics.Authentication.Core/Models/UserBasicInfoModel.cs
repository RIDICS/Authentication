using System.Collections.Generic;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class UserBasicInfoModel
    {
        public int Id { get; set; }

        public IList<UserDataBasicInfoModel> UserData { get; set; }

        public IList<UserContactBasicInfoModel> UserContacts { get; set; }
    }

    public class UserDataBasicInfoModel
    {
        public string UserDataType { get; set; }

        public string Value { get; set; }

        public List<UserDataBasicInfoModel> ChildrenUserData { get; set; }
    }

    public class UserContactBasicInfoModel
    {
        public int UserId { get; set; }
        
        public ContactTypeEnumModel ContactType { get; set; }

        public string Value { get; set; }
    }
}