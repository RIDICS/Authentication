using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UriTypeModelProfile : Profile
    {
        public UriTypeModelProfile()
        {
            CreateMap<UriTypeEntity, UriTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UriTypeValue, opt => opt.MapFrom(src => src.Value));

            CreateMap<UriTypeModel, UriTypeEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.UriTypeValue))
                .ForMember(dest => dest.UriEntities, opt => opt.Ignore());
        }
    }
}