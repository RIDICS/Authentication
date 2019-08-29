using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.ExternalIdentity;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.MapperProfiles;
using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.ContactConfirm;
using Ridics.Authentication.Core.Utils.CodeGenerator.User;
using Ridics.Authentication.Core.Utils.Formatter;
using Ridics.Authentication.Core.Utils.Hashing;
using Ridics.Authentication.Core.Utils.Validator;
using Ridics.Authentication.DataEntities;
using Ridics.Core.Shared.Providers;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core
{
    public static class IocAuthorizationCoreExtensions
    {
        public static void RegisterAuthorizationCore(this IServiceCollection services)
        {
            services.RegisterManagers();
            services.RegisterCodeGenerators();
            services.RegisterMapperProfiles();
            services.RegisterDataEntitiesComponents();
            services.RegisterConfiguration();
            services.RegisterExternalIdentityResolvers();
            services.RegisterContactValidators();
            services.RegisterHashers();
            services.RegisterContactFormatters();
            services.RegisterUtils();
        }

        private static void RegisterManagers(this IServiceCollection services)
        {
            services.AddScoped<ApiResourceManager>();
            services.AddScoped<ClaimManager>();
            services.AddScoped<ClaimTypeManager>();
            services.AddScoped<ClientManager>();
            services.AddScoped<CodeGeneratorManager>();
            services.AddScoped<DataSourceManager>();
            services.AddScoped<DynamicModuleManager>();
            services.AddScoped<ExternalIdentityManager>();
            services.AddScoped<ExternalLoginManager>();
            services.AddScoped<ExternalLoginProviderManager>();
            services.AddScoped<FileResourceManager>();
            services.AddScoped<GrantTypeManager>();
            services.AddScoped<IdentityResourceManager>();
            services.AddScoped<IFileResourceManager, LocalFileResourceManager>();
            services.AddScoped<IUserDataTypeManager, UserDataTypeManager>();
            services.AddScoped<MessageSenderManager>();
            services.AddScoped<NonceManager>();
            services.AddScoped<PermissionManager>();
            services.AddScoped<PersistedGrantManager>();
            services.AddScoped<ResourcePermissionManager>();
            services.AddScoped<ResourcePermissionTypeActionManager>();
            services.AddScoped<ResourcePermissionTypeManager>();
            services.AddScoped<RoleManager>();
            services.AddScoped<ScopeManager>();
            services.AddScoped<SecretManager>();
            services.AddScoped<UriManager>();
            services.AddScoped<UserContactManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<HashManager>();
            services.AddScoped<ContactManager>();

            services.AddTransient<ApiAccessKeyManager>();
        }

        private static void RegisterExternalIdentityResolvers(this IServiceCollection services)
        {
            services.AddScoped<ConcreteExternalIdentityResolverFactory>();
            services.AddScoped<IExternalIdentityResolver, ExternalIdentityResolver>();
        }

        private static void RegisterCodeGenerators(this IServiceCollection services)
        {
            services.AddScoped<IGenericCodeGenerator, CryptographyCodeGenerator>();
            services.AddScoped<VerificationCodeGenerator>();
            services.AddScoped<UsernameGenerator>();
            services.AddScoped<PasswordGenerator>();
            services.AddScoped<IContactConfirmCodeGenerator, EmailConfirmCodeGenerator>();
            services.AddScoped<IContactConfirmCodeGenerator, SmsConfirmCodeGenerator>();
        }

        public static void RegisterMapperProfiles(this IServiceCollection services)
        {
            services.AddScoped<Profile, ApiAccessKeyPermissionProfile>();
            services.AddScoped<Profile, ApiAccessKeyProfile>();
            services.AddScoped<Profile, ClaimModelProfile>();
            services.AddScoped<Profile, ClaimTypeEnumModelProfile>();
            services.AddScoped<Profile, ClaimTypeModelProfile>();
            services.AddScoped<Profile, ClientModelProfile>();
            services.AddScoped<Profile, ContactTypeProfile>();
            services.AddScoped<Profile, DataSourceProfile>();
            services.AddScoped<Profile, DynamicModuleBlobProfile>();
            services.AddScoped<Profile, DynamicModuleProfile>();
            services.AddScoped<Profile, ExternalIdentityModelProfile>();
            services.AddScoped<Profile, ExternalLoginProviderModelProfile>();
            services.AddScoped<Profile, ExternalUserLoginModelProfile>();
            services.AddScoped<Profile, FileResourceModelProfile>();
            services.AddScoped<Profile, GrantTypeModelProfile>();
            services.AddScoped<Profile, LevelOfAssuranceProfile>();
            services.AddScoped<Profile, PermissionModelProfile>();
            services.AddScoped<Profile, PersistedGrantModelProfile>();
            services.AddScoped<Profile, ResourceModelProfile>();
            services.AddScoped<Profile, ResourcePermissionModelProfile>();
            services.AddScoped<Profile, ResourcePermissionTypeActionModelProfile>();
            services.AddScoped<Profile, ResourcePermissionTypeModelProfile>();
            services.AddScoped<Profile, RoleModelProfile>();
            services.AddScoped<Profile, ScopeModelProfile>();
            services.AddScoped<Profile, SecretModelProfile>();
            services.AddScoped<Profile, TwoFactorLoginModelProfile>();
            services.AddScoped<Profile, UriModelProfile>();
            services.AddScoped<Profile, UriTypeModelProfile>();
            services.AddScoped<Profile, UserContactModelProfile>();
            services.AddScoped<Profile, UserDataModelProfile>();
            services.AddScoped<Profile, UserDataTypeModelProfile>();
            services.AddScoped<Profile, UserExternalIdentityModelProfile>();
            services.AddScoped<Profile, UserModelProfile>();
        }

        private static void RegisterConfiguration(this IServiceCollection services)
        {
            services.TryAddScoped<EmailConfirmConfiguration>();
            services.TryAddScoped<SmsConfirmConfiguration>();
            services.TryAddScoped<UsernameConfiguration>();
            services.TryAddScoped<PasswordConfiguration>();
            services.TryAddScoped<VerificationCodeConfiguration>();

            services.TryAddScoped<CodeGeneratorConfiguration>();
            services.TryAddScoped<IPaginationConfiguration, DefaultPaginationConfiguration>();
        }

        private static void RegisterContactValidators(this IServiceCollection services)
        {
            services.AddScoped<ContactValidatorManager>();
            services.AddScoped<UniqueContactValidator>();
            services.AddScoped<IContactValidator, PhoneContactValidator>();
            services.AddScoped<IContactValidator, EmailContactValidator>();
        }

        private static void RegisterHashers(this IServiceCollection services)
        {
            services.AddScoped<IHasher, Sha256Hasher>();
        }

        private static void RegisterContactFormatters(this IServiceCollection services)
        {
            services.AddScoped<ContactFormatterManager>();
            services.AddScoped<IContactFormatter, PhoneNumberFormatter>();
            services.AddScoped<IContactFormatter, EmailFormatter>();
        }

        private static void RegisterUtils(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        }
    }
}
