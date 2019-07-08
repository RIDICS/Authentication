using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Account;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ExternalLoginProviderProfile : Profile
    {
        public ExternalLoginProviderProfile()
        {
            CreateMap<ExternalLoginProviderModel, ExternalLoginProviderViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Enable, opt => opt.MapFrom(src => src.Enable))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.DisableManagingByUser, opt => opt.MapFrom(src => src.DisableManagingByUser))
                .ForMember(dest => dest.AuthenticationScheme, opt => opt.MapFrom(src => src.AuthenticationScheme))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
                .ForMember(dest => dest.MainColor, opt => opt.MapFrom(src => src.MainColor))
                .ForMember(dest => dest.LogoFileName, opt => opt.Ignore());
        }
    }
}
