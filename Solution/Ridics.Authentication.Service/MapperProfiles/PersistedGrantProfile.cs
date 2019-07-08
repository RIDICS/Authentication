using AutoMapper;
using IdentityServer4.Models;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class PersistedGrantProfile : Profile
    {
        public PersistedGrantProfile()
        {
            CreateMap<PersistedGrantModel, PersistedGrant>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.User.Id.ToString()))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client.Name.ToString()))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime))
                .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.ExpirationTime))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));

            CreateMap<PersistedGrant, PersistedGrantModel>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserModel {Id = int.Parse(src.SubjectId)}))
                .ForMember(dest => dest.Client, opt => opt.MapFrom(src => new ClientModel {Name = src.ClientId}))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.CreationTime))
                .ForMember(dest => dest.ExpirationTime, opt => opt.MapFrom(src => src.Expiration))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}