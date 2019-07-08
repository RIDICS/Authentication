using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ScopeModelProfile : Profile
    {
        public ScopeModelProfile()
        {
            CreateMap<ScopeEntity, ScopeModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.ClaimTypes.ToList()))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument));
        }
    }
}