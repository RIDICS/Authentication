using System.Security.Claims;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ClaimModelProfile : Profile
    {
        public ClaimModelProfile()
        {
            CreateMap<ClaimEntity, Claim>()
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ConstructUsing(src => new Claim(src.ClaimType.Name, src.Value, src.ClaimType.Type.Name));

            CreateMap<ClaimEntity, ClaimModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ClaimType))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}