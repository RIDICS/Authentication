using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ResourceModelProfile : Profile
    {
        public ResourceModelProfile()
        {
            CreateMap<ResourceEntity, ResourceModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.ClaimTypes))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument));

            CreateMap<ApiResourceEntity, ApiResourceModel>()
                .IncludeBase<ResourceEntity, ResourceModel>()
                .ForMember(dest => dest.Scopes, opt => opt.MapFrom(src => src.Scopes))
                .ForMember(dest => dest.ApiSecrets, opt => opt.MapFrom(src => src.ApiSecrets));

            CreateMap<IdentityResourceEntity, IdentityResourceModel>()
                .IncludeBase<ResourceEntity, ResourceModel>();
        }
    }
}