using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Validators
{
    public class ValidationOptions
    {
        public int? UserId { get; set; }
        public LevelOfAssuranceEnum MinLevelOfAssuranceToValidateAgainst { get; set; }
    }
}