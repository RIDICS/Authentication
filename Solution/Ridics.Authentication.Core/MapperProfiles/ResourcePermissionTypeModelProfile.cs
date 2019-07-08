using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ResourcePermissionTypeModelProfile : Profile
    {
        public ResourcePermissionTypeModelProfile()
        {
            CreateMap<ResourcePermissionTypeEntity, ResourcePermissionTypeInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ResourcePermissionTypeEntity, ResourcePermissionTypeModel>()
                .IncludeBase<ResourcePermissionTypeEntity, ResourcePermissionTypeInfoModel>()
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions));

            CreateMap<ResourcePermissionTypeModel, ResourcePermissionTypeInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)).ReverseMap();
        }
    }
}