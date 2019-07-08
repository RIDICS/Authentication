using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile(IPermissionSorter permissionSorter, IResourcePermissionTypeActionSorter resourcePermissionTypeActionSorter,
            IResourcePermissionSorter resourcePermissionSorter)
        {
            CreateMap<RoleInfoModel, ApplicationRole>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedName, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore());

            CreateMap<ApplicationRole, RoleInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AuthenticationServiceOnly, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ApplicationRole, RoleModel>()
                .IncludeBase<ApplicationRole, RoleInfoModel>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                });

            CreateMap<RoleModel, ApplicationRole>()
                .IncludeBase<RoleInfoModel, ApplicationRole>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                });

            CreateMap<RoleInfoModel, RoleViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<RoleModel, RoleViewModel>()
                .IncludeBase<RoleInfoModel, RoleViewModel>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                });

            CreateMap<RoleViewModel, RoleModel>()
                .IncludeBase<RoleViewModel, RoleInfoModel>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.AuthenticationServiceOnly, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                });

            CreateMap<ApplicationRole, RoleViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                })
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.Permissions = permissionSorter.SortPermissions(dest.Permissions);
                    dest.ResourcePermissions = resourcePermissionSorter.SortResourcePermissions(dest.ResourcePermissions);
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                });
        }
    }
}