using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class ExternalLoginProviderContractProfile : Profile
    {
        public ExternalLoginProviderContractProfile()
        {
            CreateMap<ExternalLoginProviderModel, ExternalLoginProviderContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Enable, opt => opt.MapFrom(src => src.Enable))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.DisableManagingByUser, opt => opt.MapFrom(src => src.DisableManagingByUser))
                .ForMember(dest => dest.AuthenticationScheme, opt => opt.MapFrom(src => src.AuthenticationScheme))
                .ForMember(dest => dest.LogoResourceId, opt => opt.MapFrom(src => src.Logo.Id))
                .ForMember(dest => dest.MainColor, opt => opt.MapFrom(src => src.MainColor))
                .ForMember(dest => dest.DescriptionLocalizationKey, opt => opt.MapFrom(src => src.DescriptionLocalizationKey));
        }
    }
}
