using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UserContactModelProfile : Profile
    {
        public UserContactModelProfile()
        {
            CreateMap<UserContactEntity, UserContactModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.ConfirmCode, opt => opt.MapFrom(src => src.ConfirmCode))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.LevelOfAssurance, opt => opt.MapFrom(src => src.LevelOfAssurance))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource));

            CreateMap<UserContactEntity, UserContactBasicInfoModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.ContactType, opt => opt.MapFrom(src => src.Type));
        }
    }
}
