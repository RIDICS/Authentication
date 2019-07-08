using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class RoleModelProfile : Profile
    {
        public RoleModelProfile()
        {
            CreateMap<RoleEntity, RoleInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AuthenticationServiceOnly, opt => opt.MapFrom(src => src.AuthenticationServiceOnly))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
  

            CreateMap<RoleEntity, RoleModel>()
                .IncludeBase<RoleEntity, RoleInfoModel>()
                .ForMember(dest => dest.ResourcePermissions, opt => opt.MapFrom(src => src.ResourcePermissions))
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

            CreateMap<RoleModel, RoleInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AuthenticationServiceOnly, opt => opt.MapFrom(src => src.AuthenticationServiceOnly))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ReverseMap();
        }
    }
}