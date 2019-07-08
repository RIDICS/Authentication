using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Account;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ExternalLoginProfile : Profile
    {
        public ExternalLoginProfile()
        {
            CreateMap<ExternalUserLoginModel, ExternalLoginViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.ProviderKey))
                .ForMember(dest => dest.LoginProvider, opt => opt.MapFrom(src => src.LoginProvider))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.ProviderKey))
                .ForMember(dest => dest.LoginProvider, opt => opt.MapFrom(src => src.LoginProvider));
        }
    }
}