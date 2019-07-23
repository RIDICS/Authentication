using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class PermissionContractProfile : Profile
    {
        public PermissionContractProfile(IRoleSorter roleSorter)
        {
            CreateMap<PermissionInfoModel, PermissionContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<PermissionInfoModel, PermissionContract>()
                .IncludeBase<PermissionInfoModel, PermissionContractBase>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<PermissionModel, PermissionContract>()
                .IncludeBase<PermissionInfoModel, PermissionContract>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) => { dest.Roles = roleSorter.SortRoles(dest.Roles); });

            CreateMap<PermissionContractBase, PermissionInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<PermissionContract, PermissionInfoModel>()
                .IncludeBase<PermissionContractBase, PermissionInfoModel>();

            CreateMap<PermissionContract, PermissionModel>()
                .IncludeBase<PermissionContract, PermissionInfoModel>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
                .AfterMap((src, dest) => { dest.Roles = roleSorter.SortRoles(dest.Roles); });

        }
    }
}