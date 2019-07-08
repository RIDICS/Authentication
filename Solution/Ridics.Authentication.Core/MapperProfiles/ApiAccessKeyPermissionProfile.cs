using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ApiAccessKeyPermissionProfile : Profile
    {
        public ApiAccessKeyPermissionProfile()
        {
            CreateMap<ApiAccessKeyPermissionEntity, ApiAccessKeyPermissionModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => src.Permission));
        }
    }
}