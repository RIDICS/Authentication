namespace Ridics.Authentication.Service.Models.API.UserActivation
{
    public class UserData
    {
        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's full name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// User's Master User Identifier
        /// </summary>
        public System.Guid Muid { get; set; }
    }
}