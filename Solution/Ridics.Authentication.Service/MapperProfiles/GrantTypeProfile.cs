using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class GrantTypeProfile : Profile
    {
        public GrantTypeProfile()
        {
            CreateMap<GrantTypeModel, GrantTypeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

            CreateMap<GrantTypeViewModel, string>().ConstructUsing(src => src.Value);

            CreateMap<GrantTypeModel, string>().ConstructUsing(src => src.Value);
        }
    }
}