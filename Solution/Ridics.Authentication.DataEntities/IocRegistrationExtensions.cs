using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.DataEntities.Comparer;
using Ridics.Authentication.DataEntities.Providers;
using Ridics.Authentication.DataEntities.Proxies;
using Ridics.Authentication.DataEntities.Repositories;
using Ridics.Authentication.DataEntities.UnitOfWork;
using Ridics.Authentication.DataEntities.Utils;
using Ridics.Authentication.DataEntities.Validators;
using Ridics.Authentication.DataEntities.Validators.Implementation;
using Ridics.Authentication.DataEntities.Validators.Interface;
using Ridics.Core.DataEntities.Shared.Utils;

namespace Ridics.Authentication.DataEntities
{
    public static class IocRegistrationExtensions
    {
        public static void RegisterDataEntitiesComponents(this IServiceCollection services)
        {
            services.RegisterRepositories();
            services.RegisterUnitOfWorks();
            services.RegisterValidators();
            services.RegisterMappingProviders();
            services.RegisterVersioningProxies();
            services.RegisterConvertors();
        }

        private static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ApiAccessKeyRepository>();
            services.AddSingleton<ApiResourceRepository>();
            services.AddSingleton<ClaimRepository>();
            services.AddSingleton<ClaimTypeEnumRepository>();
            services.AddSingleton<ClaimTypeRepository>();
            services.AddSingleton<ClientRepository>();
            services.AddSingleton<DataSourceRepository>();
            services.AddSingleton<DynamicModuleBlobRepository>();
            services.AddSingleton<DynamicModuleRepository>();
            services.AddSingleton<ExternalIdentityRepository>();
            services.AddSingleton<ExternalLoginProviderRepository>();
            services.AddSingleton<ExternalLoginRepository>();
            services.AddSingleton<FileResourceRepository>();
            services.AddSingleton<GrantTypeRepository>();
            services.AddSingleton<IdentityResourceRepository>();
            services.AddSingleton<LevelOfAssuranceRepository>();
            services.AddSingleton<PermissionRepository>();
            services.AddSingleton<PersistedGrantRepository>();
            services.AddSingleton<ResourcePermissionRepository>();
            services.AddSingleton<ResourcePermissionTypeActionRepository>();
            services.AddSingleton<ResourcePermissionTypeRepository>();
            services.AddSingleton<RoleRepository>();
            services.AddSingleton<ScopeRepository>();
            services.AddSingleton<SecretRepository>();
            services.AddSingleton<UriRepository>();
            services.AddSingleton<UriTypeRepository>();
            services.AddSingleton<UserContactRepository>();
            services.AddSingleton<UserDataRepository>();
            services.AddSingleton<UserDataTypeRepository>();
            services.AddSingleton<UserRepository>();
        }

        private static void RegisterUnitOfWorks(this IServiceCollection services)
        {
            services.AddSingleton<ApiAccessKeyUoW>();
            services.AddSingleton<ApiResourceUoW>();
            services.AddSingleton<ClaimTypeUoW>();
            services.AddSingleton<ClaimUoW>();
            services.AddSingleton<ClientUoW>();
            services.AddSingleton<DataSourceUoW>();
            services.AddSingleton<DynamicModuleUoW>();
            services.AddSingleton<ExternalIdentityUoW>();
            services.AddSingleton<ExternalLoginProviderUoW>();
            services.AddSingleton<FileResourceUoW>();
            services.AddSingleton<GrantTypeUoW>();
            services.AddSingleton<IdentityResourceUoW>();
            services.AddSingleton<PermissionUoW>();
            services.AddSingleton<PersistedGrantUoW>();
            services.AddSingleton<ResourcePermissionTypeActionUoW>();
            services.AddSingleton<ResourcePermissionTypeUoW>();
            services.AddSingleton<ResourcePermissionUoW>();
            services.AddSingleton<RoleUoW>();
            services.AddSingleton<ScopeUoW>();
            services.AddSingleton<SecretUoW>();
            services.AddSingleton<UriTypeUoW>();
            services.AddSingleton<UriUoW>();
            services.AddSingleton<UserContactUoW>();
            services.AddSingleton<UserDataTypeUoW>();
            services.AddSingleton<UserDataUoW>();
            services.AddSingleton<UserUoW>();
        }

        private static void RegisterValidators(this IServiceCollection services)
        {
            services.AddSingleton<IUserDataValidatorManager, UserDataValidatorManager>();
            services.AddSingleton<IUserDataValidator, UniqueUserDataValidator>();
        }

        private static void RegisterMappingProviders(this IServiceCollection services)
        {
            services.AddSingleton<IMappingProvider, AuthorizationMappingProvider>();
        }

        private static void RegisterVersioningProxies(this IServiceCollection services)
        {
            services.AddSingleton<UserContactVersioningProxy>();
            services.AddSingleton<UserDataVersioningProxy>();
            services.AddSingleton<UserDataEqualityComparer>();
            services.AddSingleton<UserContactEqualityComparer>();
        }

        private static void RegisterConvertors(this IServiceCollection services)
        {
            services.AddSingleton<UserDataStructureConvertor>();
        }
    }
}