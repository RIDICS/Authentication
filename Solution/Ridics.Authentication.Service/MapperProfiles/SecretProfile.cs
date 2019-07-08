using AutoMapper;
using IdentityServer4.Models;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class SecretProfile : Profile
    {
        public SecretProfile()
        {
            CreateMap<SecretModel, SecretViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration));

            CreateMap<SecretModel, Secret>()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.Expiration, opt => opt.Ignore())
                .ConstructUsing(src => new Secret(src.Value.Sha256(), src.Description, src.Expiration)); //TODO Hash the value before storing it to database

            CreateMap<SecretViewModel, Secret>()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Value, opt => opt.Ignore())
                .ForMember(dest => dest.Expiration, opt => opt.Ignore())
                .ConstructUsing(src => new Secret(src.Value.Sha256(), src.Description, src.Expiration)); //TODO Hash the value before storing it to database
        }
    }
}