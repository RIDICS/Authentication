using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.DynamicModule;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class DynamicModuleProfile : Profile
    {
        public DynamicModuleProfile()
        {
            CreateMap<DynamicModuleModel, CreateDynamicModuleViewModel>()
                .ForMember(dest => dest.ModuleGuid, opt => opt.MapFrom(src => src.ModuleGuid))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ConfigurationVersion, opt => opt.MapFrom(src => src.ConfigurationVersion));

            CreateMap<CreateDynamicModuleViewModel, DynamicModuleModel>()
                .ForMember(dest => dest.ModuleGuid, opt => opt.MapFrom(src => src.ModuleGuid))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.NameOption) ||
                        src.NameOption == CreateDynamicModuleViewModel.CustomName
                            ? src.Name
                            : src.NameOption
                    )
                )
                .ForMember(dest => dest.ConfigurationVersion, opt => opt.MapFrom(src => src.ConfigurationVersion));

            CreateMap<DynamicModuleModel, DynamicModuleViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModuleGuid, opt => opt.MapFrom(src => src.ModuleGuid))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ConfigurationVersion, opt => opt.MapFrom(src => src.ConfigurationVersion))
                //.ForMember(dest => dest.DynamicModuleBlobs, opt => opt.MapFrom(src => src.DynamicModuleBlobs))
                ;

            CreateMap<DynamicModuleViewModel, DynamicModuleModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ModuleGuid, opt => opt.MapFrom(src => src.ModuleGuid))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name)
                )
                .ForMember(dest => dest.ConfigurationVersion, opt => opt.MapFrom(src => src.ConfigurationVersion))
                //.ForMember(dest => dest.DynamicModuleBlobs, opt => opt.MapFrom(src => src.DynamicModuleBlobs))
                ;
        }
    }
}