using System.Security.Claims;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Users.Claims;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ClaimProfile : Profile
    {
        public ClaimProfile()
        {
            CreateMap<ClaimModel, Claim>()
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ConstructUsing(src => new Claim(src.Type.Name, src.Value, src.Type.Type.Name))
                .ReverseMap()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<ClaimModel, ClaimViewModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ReverseMap()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}