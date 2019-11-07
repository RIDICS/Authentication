using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.UserData;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Core.Shared.Types;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class UserContractProfile : Profile
    {
        public UserContractProfile(IRoleSorter roleSorter)
        {
            CreateMap<UserModel, UserContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MasterUserId, opt => opt.MapFrom(new NullableMasterUserIdDataResolver<UserContractBase>(UserDataTypes.MasterUserId)))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(UserDataTypes.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(UserDataTypes.LastName)))
                .ForMember(dest => dest.ExternalLogin, opt => opt.MapFrom(src => src.ExternalUserLogins))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(new UserContactValueResolver<UserContractBase>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(new UserContactValueResolver<UserContractBase>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserContractBase>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.PhoneNumberConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<UserContractBase>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.EmailLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserContractBase>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailConfirmCode,
                    opt => opt.MapFrom(new UserContactConfirmCodeResolver<UserContractBase>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.EmailConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<UserContractBase>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneNumberConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<UserContractBase>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.SecondName,
                    opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(CustomUserDataTypes.SecondName)))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(CustomUserDataTypes.FullName)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(new TitleUserDataResolver<UserContractBase>(CustomUserDataTypes.Title)))
                .ForMember(dest => dest.Prefix, opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(CustomUserDataTypes.Prefix)))
                .ForMember(dest => dest.Suffix, opt => opt.MapFrom(new UserDataStringValueResolver<UserContractBase>(CustomUserDataTypes.Suffix)))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) => { dest.Roles = roleSorter.SortRoles(dest.Roles); })
                .ForMember(dest => dest.ValidTwoFactorProviders, opt => opt.Ignore());

            CreateMap<UserBasicInfoModel, BasicUserInfoContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.UserData, opt => opt.MapFrom(new DictionaryUserDataBasicInfoResolver<BasicUserInfoContract>()));

            CreateMap<UserContractBase, UserModel>() //TODO decide which properties must be transferred to web hub from auth service
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.LastChange, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEndDateUtc, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationCode, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationCodeCreateTime, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalUserLogins, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorLogins, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ForMember(dest => dest.UserContacts, opt => opt.MapFrom<UserContactToListResolver<UserContractBase>>())
                .ForMember(dest => dest.UserData, opt => opt.MapFrom<UserDataToListResolver<UserContractBase>>())
                .ForMember(dest => dest.ExternalIdentities, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetTokenCreateTime, opt => opt.Ignore());

            CreateMap<UserModel, UserContract>() //TODO decide which properties must be transferred to web hub from auth service
                .IncludeBase<UserModel, UserContractBase>()
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.ValidTwoFactorProviders, opt => opt.Ignore());

            CreateMap<UserModel, UserInfoContract>()
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(new UserContactValueResolver<UserInfoContract>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(new UserContactValueResolver<UserInfoContract>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.EmailConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<UserInfoContract>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneNumberConfirmed,
                    opt => opt.MapFrom(new UserContactConfirmResolver<UserInfoContract>(ContactTypeEnumModel.Phone)))
                .ForMember(dest => dest.EmailLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserInfoContract>(ContactTypeEnumModel.Email)))
                .ForMember(dest => dest.PhoneNumberLevelOfAssurance,
                    opt => opt.MapFrom(new UserContactLevelOfAssuranceResolver<UserInfoContract>(ContactTypeEnumModel.Phone)));

            CreateMap<UserContract, UserModel>() //TODO decide which properties must be transferred to web hub from auth service
                .IncludeBase<UserContractBase, UserModel>()
                .ForMember(dest => dest.VerificationCode, opt => opt.MapFrom(src => src.VerificationCode))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider));

            CreateMap<UserModel, VerifiedUserCreatedContract
                >() //TODO decide which properties must be transferred to web hub from auth service
                .IncludeBase<UserModel, UserContractBase>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider))
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<VerifiedUserCreatedContract, UserModel
                >() //TODO decide which properties must be transferred to web hub from auth service
                .IncludeBase<UserContractBase, UserModel>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => src.TwoFactorEnabled))
                .ForMember(dest => dest.TwoFactorProvider, opt => opt.MapFrom(src => src.TwoFactorProvider));

            CreateMap<UserModel, UserWithRolesContract>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(new UserDataStringValueResolver<UserWithRolesContract>(UserDataTypes.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(new UserDataStringValueResolver<UserWithRolesContract>(UserDataTypes.LastName)))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));
        }
    }
}