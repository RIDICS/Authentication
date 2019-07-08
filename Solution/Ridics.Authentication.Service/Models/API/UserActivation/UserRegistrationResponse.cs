namespace Ridics.Authentication.Service.Models.API.UserActivation
{
    public class UserRegistrationResponse
    {
        /// <summary>
        /// Status of activation of a user at the portal
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Master User Identifier
        /// </summary>
        public System.Guid Muid { get; set; }

        /// <summary>
        /// Unique code received by user
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// User's username at the portal
        /// </summary>
        public string Username { get; set; }
    }
}