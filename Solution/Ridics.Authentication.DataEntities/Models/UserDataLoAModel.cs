using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Models
{
    public class UserDataLoAModel
    {
        public UserDataEntity UserData { get; set; }

        public LevelOfAssuranceEnum LevelOfAssuranceEnum { get; set; }
    }
}
