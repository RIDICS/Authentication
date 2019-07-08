using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.MapperProfiles.Resolvers.Uri;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class UriProfile : Profile
    {
        public UriProfile()
        {
            CreateMap<UriModel, UriViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.IsRedirect, opt => opt.MapFrom(new UriTypeViewModelResolver<UriViewModel>(UriModelHelper.Redirect)))
                .ForMember(dest => dest.IsPostLogoutRedirect,
                    opt => opt.MapFrom(new UriTypeViewModelResolver<UriViewModel>(UriModelHelper.PostLougoutRedirect)))
                .ForMember(dest => dest.IsCorsOrigin, opt => opt.MapFrom(new UriTypeViewModelResolver<UriViewModel>(UriModelHelper.CorsOrigin)))
                .ForMember(dest => dest.IsFrontChannelLogout,
                    opt => opt.MapFrom(new UriTypeViewModelResolver<UriViewModel>(UriModelHelper.FrontChannelLogout)));

            CreateMap<UriViewModel, UriModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.UriTypes, opt => opt.MapFrom(new UriTypeModelResolver<UriModel>()));

            CreateMap<UriViewModel, string>().ConvertUsing(src => src == null ? null : src.Value);

            CreateMap<UriModel, string>().ConvertUsing(src => src == null ? null : src.Value);
        }
    }
}