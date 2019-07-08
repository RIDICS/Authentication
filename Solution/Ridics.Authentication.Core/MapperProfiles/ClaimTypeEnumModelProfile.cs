using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ClaimTypeEnumModelProfile : Profile
    {
        public ClaimTypeEnumModelProfile()
        {
            CreateMap<ClaimTypeEnumModel, ClaimTypeEnumEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClaimTypes, opt => opt.Ignore());

            CreateMap<ClaimTypeEnumEntity, ClaimTypeEnumModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
        }
    }
}