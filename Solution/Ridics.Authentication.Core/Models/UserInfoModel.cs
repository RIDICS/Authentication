using System;

namespace Ridics.Authentication.Core.Models
{
    public class UserInfoModel
    {
        public int Id { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public DateTime LastChange { get; set; }

        public string SecurityStamp { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public string TwoFactorProvider { get; set; }

        public string VerificationCode { get; set; }

        public DateTime? VerificationCodeCreateTime { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenCreateTime { get; set; }
    }
}