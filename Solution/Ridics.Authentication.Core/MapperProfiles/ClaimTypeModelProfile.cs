using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ClaimTypeModelProfile : Profile
    {
        public ClaimTypeModelProfile()
        {
            CreateMap<ClaimTypeEntity, ClaimTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.SelectedType, opt => opt.Ignore())
                .ForMember(dest => dest.AllTypeValues, opt => opt.Ignore());
        }
    }
}