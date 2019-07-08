using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Utils;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ExternalLoginProviderModelProfile : Profile
    {
        public ExternalLoginProviderModelProfile(
            IExternalLoginProviderHydrator externalLoginProviderHydrator
        )
        {
            CreateMap<ExternalLoginProviderEntity, ExternalLoginProviderModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreateNotExistUser, opt => opt.MapFrom(src => src.CreateNotExistUser))
                .ForMember(dest => dest.DisableManagingByUser, opt => opt.MapFrom(src => src.DisableManagingByUser))
                .ForMember(dest => dest.HideOnLoginScreen, opt => opt.MapFrom(src => src.HideOnLoginScreen))
                .ForMember(dest => dest.AuthenticationScheme, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
                .ForMember(dest => dest.MainColor, opt => opt.MapFrom(src => src.MainColor))
                .ForMember(dest => dest.DescriptionLocalizationKey, opt => opt.MapFrom(src => src.DescriptionLocalizationKey))
                .AfterMap((src, dest) => externalLoginProviderHydrator.Hydrate(dest));
        }
    }
}
