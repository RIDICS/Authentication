namespace Ridics.Authentication.Core.Models.DataResult
{
    public class DataResultErrorCode
    {
        public const string GenericError = "generic-error";
        public const string UserNotExistId = "user-not-exist-id";
        public const string UserNotExistUsername = "user-not-exist-username";
        public const string UserNotExistEmail = "user-not-exist-email";
        public const string UserNotExistVerificationCode = "user-not-exist-verification-code";
        public const string UserNotExistExternalLogin = "user-not-exist-external-login";
        public const string UserNotExistPhone = "user-not-exist-phone";
        public const string UserNotExistUserData = "user-not-exist-user-data";
        public const string CreateUserErrorGenerateVerificationCode = "create-user-error-generate-verification-code";
        public const string CreateUserError = "create-user-error";
        public const string CreateUserErrorUserData = "create-user-error-user-data";
        public const string CreateUserErrorUserContactEmail = "create-user-error-user-contact-email";
        public const string CreateUserErrorUserContactPhone = "create-user-error-user-contact-phone";
        public const string CreateUserErrorContactEmailNotValid = "create-user-error-user-contact-email-not-valid";
        public const string CreateUserErrorContactPhoneNotValid = "create-user-error-user-contact-phone-not-valid";
        public const string CreateUserErrorExternalIdentity = "create-user-error-external-identity";
        public const string GenerateVerificationCode = "generate-verification-code";
        public const string GenerateUsername = "generate-username";
        public const string RoleNotExistName = "role-not-exist-name";
        public const string RoleNotExistId = "role-not-exist-id";
        public const string UserDataInvalidType = "user-data-invalid-type";
        public const string ClientNotExistId = "client-not-exist-id";
        public const string UriNoxExistId = "uri-nox-exist-id";
        public const string ApiResourceNotExistId = "api-resource-not-exist-id";
        public const string SecretNotExistId = "secret-not-exist-id";
        public const string PermissionNotExistId = "permission-not-exist-id";
        public const string ResourceNotExistId = "resource-not-exist-id";
        public const string ClaimTypeNotExistId = "claim-type-not-exist-id";
        public const string ClaimTypeEnumNotExistId = "claim-type-enum-not-exist-id";
        public const string ClaimNothingToRemove = "claim-nothing-to-remove";
        public const string ApiResourceNotExistName = "api-resource-not-exist-name";
        public const string FileNotExistId = "file-not-exist-id";
        public const string TwoFactorValidationError = "two-factor-validation-error";
        public const string EmailNotValid = "email-not-valid";
        public const string PhoneNumberNotValid = "phone-not-valid";
        public const string SaveUserErrorContactEmailNotValid = "save-user-error-user-contact-email-not-valid";
        public const string SaveUserErrorContactPhoneNotValid = "save-user-error-user-contact-phone-not-valid";
        public const string SaveUserErrorContactEmailNotUnique = "save-user-error-user-contact-email";
        public const string SaveUserErrorContactPhoneNumberNotUnique = "save-user-error-user-contact-phone";
        public const string EmailNotUnique = "email-not-unique";
        public const string PhoneNumberNotUnique = "phone-not-unique";
        public const string GeneratePassword = "generate-password";
        public const string PermissionTypeNotExistId = "permission-type-not-exist-id";
        public const string DynamicModuleNotExistId = "dynamic-module-not-exist-id";
        public const string ApiAccessKeyNotExistId = "api-access-key-not-exist-id";
        public const string BadRequest400 = "bad-request-code-400";
        public const string Unauthorized401 = "unauthorized-code-401";
        public const string Forbidden403 = "forbidden-code-403";
        public const string NotFound404 = "not-found-code-404";
        public const string InternalServerError500 = "internal-server-error-code-500";
        public const string BadGateway502 = "bad-gateway-code-502";
        public const string GatewayTimeout504 = "gateway-timeout-code-504";
    }
}
