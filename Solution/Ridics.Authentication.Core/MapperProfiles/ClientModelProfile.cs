using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ClientModelProfile : Profile
    {
        public ClientModelProfile()
        {
            CreateMap<ClientEntity, ClientInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DisplayUrl, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .ForMember(dest => dest.AllowAccessTokensViaBrowser, opt => opt.MapFrom(src => src.AllowAccessTokensViaBrowser));

            CreateMap<ClientEntity, ClientModel>()
                .IncludeBase<ClientEntity, ClientInfoModel>()
                .ForMember(dest => dest.Secrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(dest => dest.AllowedGrantTypes, opt => opt.MapFrom(src => src.AllowedGrantTypes))
                .ForMember(dest => dest.UriList, opt => opt.MapFrom(src => src.UriList))
                .ForMember(dest => dest.AllowedIdentityResources, opt => opt.MapFrom(src => src.AllowedIdentityResources))
                .ForMember(dest => dest.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes));

            CreateMap<ClientModel, ClientInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DisplayUrl, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .ForMember(dest => dest.AllowAccessTokensViaBrowser, opt => opt.MapFrom(src => src.AllowAccessTokensViaBrowser)).ReverseMap();
        }
    }
}