using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class PersistedGrantModelProfile : Profile
    {
        public PersistedGrantModelProfile()
        {
            CreateMap<PersistedGrantEntity, PersistedGrantModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime))
                .ForMember(dest => dest.ExpirationTime, opt => opt.MapFrom(src => src.ExpirationTime))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));
        }

    }
}