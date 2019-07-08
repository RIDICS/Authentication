using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.MapperProfiles;
using Ridics.Authentication.Service.MapperProfiles.Contracts;
using Ridics.Authentication.Service.MapperProfiles.Converters;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocAutomapperRegistrationExtensions
    {
        public static void RegisterAutomapper(this IServiceCollection services)
        {
            services.AddScoped<IMapper>((c) => Mapper.Instance);
            services.RegisterMapperProfiles();
            services.RegisterAutomapperResolvers();
            services.RegisterAutomapperConverters();
            services.RegisterSorters();
        }

        private static void RegisterAutomapperResolvers(this IServiceCollection services)
        {
            services.AddScoped<UserDataToListResolver<IConvertibleToUserData>>();
            services.AddScoped<UserContactConfirmCodeResolver<IConvertibleToUserContacts>>();
        }

        private static void RegisterAutomapperConverters(this IServiceCollection services)
        {
            services.AddScoped<DateTimeOffsetToUtcDateTimeConverter>();
            services.AddScoped<UtcDateTimeToDateTimeOffsetConverter>();
        }

        private static void RegisterMapperProfiles(this IServiceCollection services)
        {
            services.AddScoped<Profile, ApiAccessKeyProfile>();
            services.AddScoped<Profile, ClaimProfile>();
            services.AddScoped<Profile, ClaimTypeEnumProfile>();
            services.AddScoped<Profile, ClaimTypeProfile>();
            services.AddScoped<Profile, ClientProfile>();
            services.AddScoped<Profile, DateTimeProfile>();
            services.AddScoped<Profile, DynamicModuleProfile>();
            services.AddScoped<Profile, ExternalLoginProfile>();
            services.AddScoped<Profile, ExternalLoginProviderProfile>();
            services.AddScoped<Profile, GrantTypeProfile>();
            services.AddScoped<Profile, PermissionProfile>();
            services.AddScoped<Profile, PersistedGrantProfile>();
            services.AddScoped<Profile, ResourcePermissionProfile>();
            services.AddScoped<Profile, ResourcePermissionTypeActionProfile>();
            services.AddScoped<Profile, ResourcePermissionTypeProfile>();
            services.AddScoped<Profile, ResourceProfile>();
            services.AddScoped<Profile, RoleProfile>();
            services.AddScoped<Profile, ScopeProfile>();
            services.AddScoped<Profile, SecretProfile>();
            services.AddScoped<Profile, TwoFactorLoginProfile>();
            services.AddScoped<Profile, UriProfile>();
            services.AddScoped<Profile, UserProfile>();

            RegisterMapperContractProfiles(services);
        }

        private static void RegisterMapperContractProfiles(IServiceCollection services)
        {
            services.AddTransient<Profile, PermissionContractProfile>();
            services.AddTransient<Profile, RoleContractProfile>();
            services.AddTransient<Profile, UserContactContractProfile>();
            services.AddTransient<Profile, UserContractProfile>();
            services.AddTransient<Profile, ExternalLoginProviderContractProfile>();
            services.AddTransient<Profile, ExternalLoginContractProfile>();
        }

        private static void RegisterSorters(this IServiceCollection services)
        {
            services.AddScoped<IApiResourceSorter, ApiResourceByNameSorter>();
            services.AddScoped<IApiSecretSorter, ApiSecretByValueSorter>();
            services.AddScoped<IClaimSorter, ClaimByValueSorter>();
            services.AddScoped<IClaimTypeSorter, ClaimTypeByNameSorter>();
            services.AddScoped<IContactSorter, ContactByValueSorter>();
            services.AddScoped<IGrantTypeSorter, GrantTypeByDisplayNameSorter>();
            services.AddScoped<IIdentityResourceSorter, IdentityResourceByNameSorter>();
            services.AddScoped<IPermissionSorter, PermissionByNameSorter>();
            services.AddScoped<IResourcePermissionSorter, ResourcePermissionByResourceIdSorter>();
            services.AddScoped<IResourcePermissionTypeActionSorter, ResourcePermissionTypeActionByNameSorter>();
            services.AddScoped<IRoleSorter, RoleByNameSorter>();
            services.AddScoped<IScopeSorter, ScopeByNameSorter>();
            services.AddScoped<ISecretSorter, SecretByValueSorter>();
            services.AddScoped<IUriSorter, UriByValueSorter>();
            services.AddScoped<IUserSorter, UserByLastAndFirstNameSorter>();
            services.AddScoped<IUserInfoSorter, UserInfoByUsernameSorter>();
            services.AddScoped<IUserDataSorter, UserDataByValueSorter>();
        }
    }
}
