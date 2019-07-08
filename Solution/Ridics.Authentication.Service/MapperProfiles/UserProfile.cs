using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.UserData;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.API.UserActivation;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Core.Shared.Types;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile(IRoleSorter roleSorter, IResourcePermissionSorter resourcePermissionSorter,
            IResourcePermissionTypeActionSorter resourcePermissionTypeActionSorter, IClaimTypeSorter claimTypeSorter,
            IUserDataSorter userDataSorter, IContactSorter contactSorter, IClaimSorter claimSorter)
        {
            CreateMap<UserInfoModel, UserViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime));

            CreateMap<UserModel, UserViewModel>()
                .IncludeBase<UserInfoModel, UserViewModel>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(UserDataTypes.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(UserDataTypes.LastName)))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(new UserContactValueResolver<UserViewModel>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserViewModel>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<UserViewModel>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(new UserContactValueResolver<UserViewModel>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.PhoneLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserViewModel>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.PhoneNumberConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<UserViewModel>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.MapFrom(src => src.TwoFactorLogins))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.SecondName,
                    opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(CustomUserDataTypes.SecondName)))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(CustomUserDataTypes.FullName)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(new TitleUserDataResolver<UserViewModel>(CustomUserDataTypes.Title)))
                .ForMember(dest => dest.Prefix, opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(CustomUserDataTypes.Prefix)))
                .ForMember(dest => dest.Suffix, opt => opt.MapFrom(new UserDataStringValueResolver<UserViewModel>(CustomUserDataTypes.Suffix)))
                .ForMember(dest => dest.TwoFactorProviders, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                });

            CreateMap<UserViewModel, UserInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange))
                .ConstructUsingServiceLocator();

            CreateMap<UserViewModel, UserModel>()
                .IncludeBase<UserViewModel, UserInfoModel>()
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.MapFrom(src => src.TwoFactorLogins))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.UserClaims, opt => opt.MapFrom(src => src.UserClaims))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom<UserDataToListResolver<UserViewModel>>())
                .ForMember(dest => dest.UserContacts, opt => opt.MapFrom<UserContactToListResolver<UserViewModel>>())
                .ForMember(dest => dest.ExternalIdentities, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetTokenCreateTime, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                    dest.UserClaims = claimSorter.SortClaims(dest.UserClaims);
                    dest.UserData = userDataSorter.SortUserData(dest.UserData);
                    dest.UserContacts = contactSorter.SortContacts(dest.UserContacts);
                })
                .ConstructUsingServiceLocator();

            CreateMap<UserModel, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MasterUserId, opt => opt.MapFrom(new MasterUserIdDataResolver<ApplicationUser>(CustomUserDataTypes.MasterUserId)))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(new UserContactValueResolver<ApplicationUser>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<ApplicationUser>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<ApplicationUser>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(new UserContactValueResolver<ApplicationUser>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.PhoneNumberConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<ApplicationUser>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.PhoneNumberConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<ApplicationUser>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.MapFrom(src => src.TwoFactorLogins))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime))
                .ForMember(dest => dest.ExternalUserLogins, opt => opt.MapFrom(src => src.ExternalUserLogins))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(src => src.UserData))
                .ForMember(dest => dest.AuthenticatorKey, opt => opt.Ignore())
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.Roles == null)
                    {
                        dest.Permissions = null;
                        return;
                    }

                    var permissionNames = new HashSet<string>();

                    foreach (var role in src.Roles)
                    {
                        foreach (var permission in role.Permissions)
                        {
                            permissionNames.Add(permission.Name);
                        }
                    }

                    dest.Permissions = permissionNames.OrderBy(x => x).ToList();

                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                    dest.UserData = userDataSorter.SortUserData(dest.UserData);
                });

            CreateMap<ApplicationUser, UserModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEnd))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.MapFrom(src => src.TwoFactorLogins))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.UserClaims, opt => opt.MapFrom(src => src.UserClaims))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(src => src.UserData))
                .ForMember(dest => dest.UserContacts, opt => opt.MapFrom<UserContactToListResolver<ApplicationUser>>())
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange))
                .ForMember(dest => dest.ExternalIdentities, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetTokenCreateTime, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                    dest.UserClaims = claimSorter.SortClaims(dest.UserClaims);
                    dest.UserData = userDataSorter.SortUserData(dest.UserData);
                    dest.UserContacts = contactSorter.SortContacts(dest.UserContacts);
                });

            CreateMap<UserModel, UserData>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(new UserDataStringValueResolver<UserData>(UserDataTypes.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(new UserDataStringValueResolver<UserData>(UserDataTypes.LastName)))
                .ForMember(dest => dest.FullName, opt => opt.Ignore())
                .ForMember(dest => dest.EmailAddress,
                    opt => opt.MapFrom(new UserContactValueResolver<UserData>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(new UserContactValueResolver<UserData>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.Muid, opt => opt.Ignore());
        }
    }
}