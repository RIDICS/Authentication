using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ResourcePermissionProfile : Profile
    {
        public ResourcePermissionProfile(IRoleSorter roleSorter, IUserInfoSorter userInfoSorter, IUserSorter userSorter)
        {
            CreateMap<ResourcePermissionInfoModel, ResourcePermissionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourcePermissionTypeAction, opt => opt.MapFrom(src => src.ResourceTypeAction))
                .ForMember(dest => dest.ResourcePermissionString, opt => opt.Ignore());

            CreateMap<ResourcePermissionModel, ResourcePermissionViewModel>()
                .IncludeBase<ResourcePermissionInfoModel, ResourcePermissionViewModel>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userSorter.SortUsers(dest.Users);
                });

            CreateMap<ResourcePermissionViewModel, ResourcePermissionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourceTypeAction, opt => opt.MapFrom(src => src.ResourcePermissionTypeAction));

            CreateMap<ResourcePermissionViewModel, ResourcePermissionModel>()
                .IncludeBase<ResourcePermissionViewModel, ResourcePermissionInfoModel>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userInfoSorter.SortUserInfos(dest.Users);
                });

            CreateMap<ResourcePermissionViewModel, ResourcePermissionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ResourceId, opt => opt.MapFrom(src => src.ResourceId))
                .ForMember(dest => dest.ResourcePermissionTypeAction, opt => opt.MapFrom(src => src.ResourcePermissionTypeAction))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .ForMember(dest => dest.ResourcePermissionString, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userSorter.SortUsers(dest.Users);
                });

            CreateMap<ResourcePermissionViewModel, EditResourcePermissionViewModel>()
                .IncludeBase<ResourcePermissionViewModel, ResourcePermissionViewModel>()
                .ForMember(dest => dest.ResourcePermissionTypeActionList, opt => opt.Ignore());
        }
    }
}