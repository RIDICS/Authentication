using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UriModelProfile : Profile
    {
        public UriModelProfile()
        {
            CreateMap<UriEntity, UriModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Uri))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Client.Id))
                .ForMember(dest => dest.UriTypes, opt => opt.MapFrom(src => src.UriTypes));

            CreateMap<UriModel, UriEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Uri, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.UriTypes, opt => opt.MapFrom(src => src.UriTypes))
                .ForMember(dest => dest.Client, opt => opt.Ignore());

        }
    }
}