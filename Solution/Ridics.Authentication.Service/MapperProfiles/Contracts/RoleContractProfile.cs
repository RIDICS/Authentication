using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class RoleContractProfile : Profile
    {
        public RoleContractProfile(IPermissionSorter permissionSorter)
        {
            CreateMap<RoleInfoModel, RoleContractBase>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<RoleInfoModel, RoleContract>()
                .IncludeBase<RoleInfoModel, RoleContractBase>()
                .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<RoleModel, RoleContractBase>()
                .IncludeBase<RoleInfoModel, RoleContractBase>();

            CreateMap<RoleModel, RoleContract>()
                .IncludeBase<RoleInfoModel, RoleContract>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .AfterMap((src, dest) => { dest.Permissions = permissionSorter.SortPermissions(dest.Permissions); });


            CreateMap<RoleContractBase, RoleInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AuthenticationServiceOnly, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<RoleContractBase, RoleModel>()
                .IncludeBase<RoleContractBase, RoleInfoModel>();

            CreateMap<RoleContract, RoleModel>()
                .IncludeBase<RoleContractBase, RoleModel>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .AfterMap((src, dest) => { dest.Permissions = permissionSorter.SortPermissions(dest.Permissions); });
        }
    }
}