using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class DynamicModuleProfile : Profile
    {
        public DynamicModuleProfile()
        {
            CreateMap<DynamicModuleEntity, DynamicModuleModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModuleGuid, opt => opt.MapFrom(src => src.ModuleGuid))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ConfigurationVersion, opt => opt.MapFrom(src => src.ConfigurationVersion))
                .ForMember(dest => dest.ConfigurationString, opt => opt.MapFrom(src => src.Configuration))
                .ForMember(dest => dest.ConfigurationChecksum, opt => opt.MapFrom(src => src.ConfigurationChecksum))
                .ForMember(dest => dest.DynamicModuleBlobs, opt => opt.MapFrom(src => src.DynamicModuleBlobs));
        }
    }
}