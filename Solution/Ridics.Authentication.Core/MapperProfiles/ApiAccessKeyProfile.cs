using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ApiAccessKeyProfile : Profile
    {
        public ApiAccessKeyProfile()
        {
            CreateMap<ApiAccessKeyEntity, ApiAccessKeyModel>()
                .ForMember(dest => dest.ApiKeyHash, opt => opt.MapFrom(src => src.ApiKeyHash))
                .ForMember(dest => dest.HashAlgorithm, opt => opt.MapFrom(src => src.HashAlgorithm))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}