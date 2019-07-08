using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UserDataModelProfile : Profile
    {
        public UserDataModelProfile()
        {
            CreateMap<UserDataEntity, UserDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.ChildrenUserData, opt => opt.MapFrom(src => src.ChildrenUserData))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.UserDataType, opt => opt.MapFrom(src => src.UserDataType))
                .ForMember(dest => dest.ActiveFrom, opt => opt.MapFrom(src => src.ActiveFrom))
                .ForMember(dest => dest.ActiveTo, opt => opt.MapFrom(src => src.ActiveTo))
                .ForMember(dest => dest.VerifiedBy, opt => opt.MapFrom(src => src.VerifiedBy))
                .ForMember(dest => dest.LevelOfAssurance, opt => opt.MapFrom(src => src.LevelOfAssurance))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource));

            CreateMap<UserDataEntity, UserDataBasicInfoModel>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.UserDataType, opt => opt.MapFrom(src => src.UserDataType.DataTypeValue))
                .ForMember(dest => dest.ChildrenUserData, opt => opt.MapFrom(src => src.ChildrenUserData));
        }
    }
}
