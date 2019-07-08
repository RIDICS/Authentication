using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ResourcePermissionModelProfile : Profile
    {
        public ResourcePermissionModelProfile()
        {
            CreateMap<ResourcePermissionEntity, ResourcePermissionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourceTypeAction, opt => opt.MapFrom(src => src.ResourceTypeAction));


            CreateMap<ResourcePermissionEntity, ResourcePermissionModel>()
                .IncludeBase<ResourcePermissionEntity, ResourcePermissionInfoModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users));

            CreateMap<ResourcePermissionModel, ResourcePermissionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourceTypeAction, opt => opt.MapFrom(src => src.ResourceTypeAction)).ReverseMap();
        }
    }
}