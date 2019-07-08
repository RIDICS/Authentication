using System;

namespace Ridics.Authentication.Modules.Shared.Model
{
    public class ExternalLoginProviderUserModel
    {
        public LevelOfAssuranceValue<string> Email { get; set; }

        public LevelOfAssuranceValue<string> PhoneNumber { get; set; }

        public LevelOfAssuranceValue<string> FirstName { get; set; }

        public LevelOfAssuranceValue<string> LastName { get; set; }
    }
}
