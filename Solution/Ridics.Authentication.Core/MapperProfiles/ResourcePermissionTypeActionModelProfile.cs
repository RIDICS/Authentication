using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ResourcePermissionTypeActionModelProfile : Profile
    {
        public ResourcePermissionTypeActionModelProfile()
        {
            CreateMap<ResourcePermissionTypeActionEntity, ResourcePermissionTypeActionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResourcePermissionType, opt => opt.MapFrom(src => src.ResourcePermissionType));

            CreateMap<ResourcePermissionTypeActionEntity, ResourcePermissionTypeActionModel>()
                .IncludeBase<ResourcePermissionTypeActionEntity, ResourcePermissionTypeActionInfoModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));

            CreateMap<ResourcePermissionTypeActionModel, ResourcePermissionTypeActionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResourcePermissionType, opt => opt.MapFrom(src => src.ResourcePermissionType)).ReverseMap();
        }
    }
}