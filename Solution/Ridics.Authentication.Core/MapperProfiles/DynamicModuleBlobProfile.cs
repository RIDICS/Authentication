using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class DynamicModuleBlobProfile : Profile
    {
        public DynamicModuleBlobProfile()
        {
            CreateMap<DynamicModuleBlobEntity, DynamicModuleBlobModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File))
                .ForMember(dest => dest.LastChange, opt => opt.MapFrom(src => src.LastChange));
        }
    }
}
