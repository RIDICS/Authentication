using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class UserContactModel
    {
        public int Id { get; set; }

        public ContactTypeEnumModel Type { get; set; }

        public string Value { get; set; }
        
        public string ConfirmCode { get; set; }

        public UserModel User { get; set; }

        public virtual LevelOfAssuranceModel LevelOfAssurance { get; set; }

        public virtual DataSourceModel DataSource { get; set; }
    }
}
