using Microsoft.AspNetCore.Identity;
using Scalesoft.Localization.AspNetCore;

namespace Ridics.Authentication.Service.Helpers
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly ILocalizationService m_localizationService;

        public LocalizedIdentityErrorDescriber(ILocalizationService localizationService)
        {
            m_localizationService = localizationService;
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = m_localizationService.Translate("default-error", "IdentityErrorDescriber")
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = m_localizationService.Translate("concurrency-failure", "IdentityErrorDescriber")
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = nameof(PasswordMismatch),
                Description = m_localizationService.Translate("password-mismatch", "IdentityErrorDescriber")
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = m_localizationService.Translate("invalid-token", "IdentityErrorDescriber")
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = nameof(LoginAlreadyAssociated),
                Description = m_localizationService.Translate("login-already-associated", "IdentityErrorDescriber")
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = m_localizationService.TranslateFormat("invalid-user-name", "IdentityErrorDescriber", userName)
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = m_localizationService.TranslateFormat("invalid-email", "IdentityErrorDescriber", email)
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = m_localizationService.TranslateFormat("duplicate-user-name", "IdentityErrorDescriber", userName)
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = m_localizationService.TranslateFormat("duplicate-email", "IdentityErrorDescriber", email)
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(InvalidRoleName),
                Description = m_localizationService.TranslateFormat("invalid-role-name", "IdentityErrorDescriber", role)
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateRoleName),
                Description = m_localizationService.TranslateFormat("duplicate-role-name", "IdentityErrorDescriber", role)
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = m_localizationService.Translate("user-already-has-password", "IdentityErrorDescriber")
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = m_localizationService.Translate("user-lockout-not-enabled", "IdentityErrorDescriber")
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserAlreadyInRole),
                Description = m_localizationService.TranslateFormat("user-already-in-role", "IdentityErrorDescriber", role)
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = nameof(UserNotInRole),
                Description = m_localizationService.TranslateFormat("user-not-in-role", "IdentityErrorDescriber", role)
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            var charactersLabel = m_localizationService.TranslatePluralization("characters", "IdentityErrorDescriber", length);
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = m_localizationService.TranslateFormat("password-too-short", "IdentityErrorDescriber", length, charactersLabel)
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = m_localizationService.Translate("password-requires-non-alphanumeric", "IdentityErrorDescriber")
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = m_localizationService.Translate("password-requires-digit", "IdentityErrorDescriber")
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = m_localizationService.Translate("password-requires-lower", "IdentityErrorDescriber")
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = m_localizationService.Translate("password-requires-upper", "IdentityErrorDescriber")
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = m_localizationService.Translate("recovery-code-redemption-failed", "IdentityErrorDescriber")
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            var charactersLabel = m_localizationService.TranslatePluralization("characters", "IdentityErrorDescriber", uniqueChars);
            var specialLabel = m_localizationService.TranslatePluralization("special", "IdentityErrorDescriber", uniqueChars);

            return new IdentityError
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = m_localizationService.TranslateFormat("password-requires-unique-chars", "IdentityErrorDescriber", uniqueChars, specialLabel, charactersLabel)
            };
        }
    }
}