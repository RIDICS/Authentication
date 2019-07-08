using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UserExternalIdentityModelProfile : Profile
    {
        public UserExternalIdentityModelProfile()
        {
            CreateMap<UserExternalIdentityEntity, UserExternalIdentityModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalIdentityType, opt => opt.MapFrom(src => src.ExternalIdentityType))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.ExternalIdentity, opt => opt.MapFrom(src => src.ExternalIdentity));
        }
    }
}