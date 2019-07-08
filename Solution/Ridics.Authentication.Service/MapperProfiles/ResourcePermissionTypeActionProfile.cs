using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ResourcePermissionTypeActionProfile : Profile
    {
        public ResourcePermissionTypeActionProfile(IRoleSorter roleSorter, IUserInfoSorter userInfoSorter, IUserSorter userSorter)
        {
            CreateMap<ResourcePermissionTypeActionInfoModel, ResourcePermissionTypeActionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResourcePermissionType, opt => opt.MapFrom(src => src.ResourcePermissionType));

            CreateMap<ResourcePermissionTypeActionViewModel, ResourcePermissionTypeActionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResourcePermissionType, opt => opt.MapFrom(src => src.ResourcePermissionType));

            CreateMap<ResourcePermissionTypeActionModel, ResourcePermissionTypeActionViewModel>()
                .IncludeBase<ResourcePermissionTypeActionInfoModel, ResourcePermissionTypeActionViewModel>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userSorter.SortUsers(dest.Users);
                });

            CreateMap<ResourcePermissionTypeActionViewModel, ResourcePermissionTypeActionModel>()
                .IncludeBase<ResourcePermissionTypeActionViewModel, ResourcePermissionTypeActionInfoModel>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userInfoSorter.SortUserInfos(dest.Users);
                });

            CreateMap<ResourcePermissionTypeActionViewModel, ResourcePermissionTypeActionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ResourcePermissionType, opt => opt.MapFrom(src => src.ResourcePermissionType))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) =>
                {
                    dest.Roles = roleSorter.SortRoles(dest.Roles);
                    dest.Users = userSorter.SortUsers(dest.Users);
                });

            CreateMap<ResourcePermissionTypeActionViewModel, EditResourcePermissionTypeActionViewModel>()
                .IncludeBase<ResourcePermissionTypeActionViewModel, ResourcePermissionTypeActionViewModel>()
                .ForMember(dest => dest.ResourcePermissionTypeList, opt => opt.Ignore());
        }
    }
}