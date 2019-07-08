using System.Security.Claims;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ClaimTypeEnumProfile : Profile
    {
        public ClaimTypeEnumProfile()
        {
            CreateMap<ClaimTypeEnumViewModel, ClaimTypeEnumModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Claim, ClaimTypeEnumModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ValueType))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}