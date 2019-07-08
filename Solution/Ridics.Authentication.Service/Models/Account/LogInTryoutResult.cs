using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Models.Account
{
    public class LogInTryoutResult
    {
        public IActionResult ActionResult { get; }

        public bool Success { get; }

        public bool SecondFactorRequired { get; }

        public ApplicationUser User { get; }

        public LogInTryoutResult(
            IActionResult actionResult,
            bool success = false,
            ApplicationUser user = null,
            bool secondFactorRequired = false
        )
        {
            ActionResult = actionResult;
            Success = success;
            User = user;
            SecondFactorRequired = secondFactorRequired;
        }
    }
}
