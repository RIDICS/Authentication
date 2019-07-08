using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Users;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class TwoFactorLoginProfile : Profile
    {
        public TwoFactorLoginProfile()
        {
            CreateMap<TwoFactorLoginModel, TwoFactorLoginViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenProvider, opt => opt.MapFrom(src => src.TokenProvider))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime));

            CreateMap<TwoFactorLoginViewModel, TwoFactorLoginModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TokenProvider, opt => opt.MapFrom(src => src.TokenProvider))
                .ForMember(dest => dest.Token, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime));
        }
    }
}