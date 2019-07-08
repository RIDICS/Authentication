using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UserModelProfile : Profile
    {
        public UserModelProfile()
        {
            CreateMap<UserEntity, UserBasicInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(src => src.UserData))
                .ForMember(dest => dest.UserContacts, opt => opt.MapFrom(src => src.UserContacts));

            CreateMap<UserEntity, UserInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.PasswordResetToken, opt => opt.MapFrom(src => src.PasswordResetToken))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime))
                .ForMember(dest => dest.PasswordResetTokenCreateTime, opt => opt.MapFrom(src => src.PasswordResetTokenCreateTime))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange));

            CreateMap<UserEntity, UserModel>()
                .IncludeBase<UserEntity, UserInfoModel>()
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.MapFrom(src => src.TwoFactorLogins))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.ExternalUserLogins, opt => opt.MapFrom(src => src.ExternalLogins))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(src => src.UserData))
                .ForMember(dest => dest.UserContacts, opt => opt.MapFrom(src => src.UserContacts))
                .ForMember(dest => dest.UserClaims, opt => opt.MapFrom(src => src.UserClaims))
                .ForMember(dest => dest.ExternalIdentities, opt => opt.MapFrom(src => src.ExternalIdentities));

            CreateMap<UserModel, UserInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.MapFrom(src => src.LockoutEndDateUtc))
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => src.LockoutEnabled))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => src.AccessFailedCount))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.MapFrom(src => src.VerificationCodeCreateTime))
                .ForMember(dest => dest.PasswordResetToken, opt => opt.MapFrom(src => src.PasswordResetToken))
                .ForMember(dest => dest.PasswordResetTokenCreateTime, opt => opt.MapFrom(src => src.PasswordResetTokenCreateTime))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange)).ReverseMap();
        }
    }
}