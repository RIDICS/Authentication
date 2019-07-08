using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class LevelOfAssuranceProfile : Profile
    {
        public LevelOfAssuranceProfile()
        {
            CreateMap<LevelOfAssuranceEntity, LevelOfAssuranceModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level));
        }
    }
}
