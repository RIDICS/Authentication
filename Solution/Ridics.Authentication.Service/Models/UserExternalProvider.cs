using System;
using System.Collections.Generic;
using System.Security.Claims;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Models
{
    public class UserExternalProvider
    {
        public ApplicationUser User { get; set; }

        public string Provider { get; set; }

        public string ProviderUserId { get; set; }

        public IList<Claim> Claims { get; set; }
    }
}
