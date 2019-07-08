using System.Linq;
using AutoMapper;
using IdentityServer4.Models;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ScopeProfile : Profile
    {
        public ScopeProfile(IClaimTypeSorter claimsTypeSorter)
        {
            CreateMap<ScopeModel, ScopeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.Claims))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument))
                .AfterMap((src, dest) =>
                {
                    dest.Claims = claimsTypeSorter.SortClaimTypes(dest.Claims);
                })
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.Claims))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument))
                .AfterMap((src, dest) =>
                {
                    dest.Claims = claimsTypeSorter.SortClaimTypes(dest.Claims);
                });

            CreateMap<ScopeViewModel, Scope>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument))
                .ForMember(dest => dest.Emphasize, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ConstructUsing(src => new Scope(src.Name, src.Claims.Select(x => x.Name)));

            CreateMap<ScopeModel, Scope>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Required))
                .ForMember(dest => dest.ShowInDiscoveryDocument, opt => opt.MapFrom(src => src.ShowInDiscoveryDocument))
                .ForMember(dest => dest.Emphasize, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ConstructUsing(src => new Scope(src.Name, src.Claims.Select(x => x.Name)));
        }
    }
}