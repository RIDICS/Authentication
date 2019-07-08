using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile(IRoleSorter roleSorter)
        {
            CreateMap<PermissionInfoModel, PermissionViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<PermissionModel, PermissionViewModel>()
                .IncludeBase<PermissionInfoModel, PermissionViewModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) => { dest.Roles = roleSorter.SortRoles(dest.Roles); });

            CreateMap<PermissionViewModel, PermissionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<PermissionViewModel, PermissionModel>()
                .IncludeBase<PermissionViewModel, PermissionInfoModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) => { dest.Roles = roleSorter.SortRoles(dest.Roles); });

        }
    }
}