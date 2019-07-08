using System;
using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class UserDataModel
    {
        public virtual int Id { get; set; }

        public virtual string Value { get; set; }

        public virtual UserModel User { get; set; }

        public virtual UserInfoModel VerifiedBy { get; set; }

        public virtual UserDataTypeModel UserDataType { get; set; }

        public virtual IList<UserDataModel> ChildrenUserData { get; set; }

        public virtual DateTime ActiveFrom { get; set; }

        public virtual DateTime? ActiveTo { get; set; }

        public virtual LevelOfAssuranceModel LevelOfAssurance { get; set; }

        public virtual DataSourceModel DataSource { get; set; }
    }
}
