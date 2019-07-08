using System;

namespace Ridics.Authentication.Service.Models.API.UserActivation
{
    public class UserActivationResponse
    {
        /// <summary>
        /// Status of activation of a user at the portal
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Master User Identifier
        /// </summary>
        public Guid? Muid { get; set; }

        /// <summary>
        /// Date and time of user activation
        /// </summary>
        public DateTime? ActivationTime { get; set; }
    }

    public class UserActivationContainerResponse
    {
        /// <summary>
        /// ID of user specified in request
        /// </summary>
        public string IdFromRequest { get; set; }

        public UserActivationResponse ActivationStatus { get; set; }
    }
}