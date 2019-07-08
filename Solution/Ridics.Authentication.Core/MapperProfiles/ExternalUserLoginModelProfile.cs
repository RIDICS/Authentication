using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ExternalUserLoginModelProfile : Profile
    {
        public ExternalUserLoginModelProfile()
        {
            CreateMap<ExternalLoginEntity, ExternalUserLoginModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.ProviderKey, opt => opt.MapFrom(src => src.ProviderKey))
                .ForMember(dest => dest.LoginProvider, opt => opt.MapFrom(src => src.Provider));
        }
    }
}