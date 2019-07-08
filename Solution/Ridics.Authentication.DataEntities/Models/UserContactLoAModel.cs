using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Models
{
    public class UserContactLoAModel
    {
        public UserContactEntity UserContact { get; set; }

        public LevelOfAssuranceEnum LevelOfAssuranceEnum { get; set; }
    }
}
