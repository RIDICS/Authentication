using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class TwoFactorLoginModelProfile : Profile
    {
        public TwoFactorLoginModelProfile()
        {
            CreateMap<TwoFactorLoginEntity, TwoFactorLoginModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenProvider, opt => opt.MapFrom(src => src.TokenProvider))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime));

            CreateMap<TwoFactorLoginModel, TwoFactorLoginEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenProvider, opt => opt.MapFrom(src => src.TokenProvider))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}