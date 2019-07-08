using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer4.Models;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Resources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile(IClaimTypeSorter claimsTypeSorter, IScopeSorter scopesSorter, IApiSecretSorter secretSorter)
        {
            CreateMap<ResourceModel, ResourceViewModel>()
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

            CreateMap<ApiResourceModel, ApiResourceViewModel>()
                .IncludeBase<ResourceModel, ResourceViewModel>()
                .ForMember(dest => dest.Scopes, opt => opt.MapFrom(src => src.Scopes))
                .ForMember(dest => dest.ApiSecrets, opt => opt.MapFrom(src => src.ApiSecrets))
                .AfterMap((src, dest) =>
                {
                    dest.Scopes = scopesSorter.SortScopes(dest.Scopes);
                    dest.ApiSecrets = secretSorter.SortApiSecrets(dest.ApiSecrets);
                });

            CreateMap<ApiResourceViewModel, ApiResourceModel>()
                .IncludeBase<ResourceViewModel, ResourceModel>()
                .ForMember(dest => dest.Scopes, opt => opt.MapFrom(src => src.Scopes))
                .ForMember(dest => dest.ApiSecrets, opt => opt.MapFrom(src => src.ApiSecrets))
                .AfterMap((src, dest) =>
                {
                    dest.Scopes = scopesSorter.SortScopes(dest.Scopes);
                    dest.ApiSecrets = secretSorter.SortApiSecrets(dest.ApiSecrets);
                });

            CreateMap<ApiResourceViewModel, ApiResource>()
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ForMember(dest => dest.Scopes, opt => opt.MapFrom(src => src.Scopes))
                .ConstructUsing(src => new ApiResource(src.Name, src.Claims.Select(x => x.Name)));

            CreateMap<IdentityResourceModel, IdentityResourceViewModel>()
                .IncludeBase<ResourceModel, ResourceViewModel>();

            CreateMap<IdentityResourceViewModel, IdentityResourceModel>()
                .IncludeBase<ResourceViewModel, ResourceModel>();

            CreateMap<IdentityResourceViewModel, IdentityResource>()
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.Emphasize, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ConstructUsing(src => new IdentityResource(src.Name, src.Claims.Select(x => x.Name)));

            CreateMap<ApiResourceModel, ApiResource>()
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ForMember(dest => dest.Scopes, opt => opt.MapFrom(src => src.Scopes))
                .ConstructUsing(src => new ApiResource(src.Name, src.Claims.Select(x => x.Name)));

            CreateMap<IdentityResourceModel, IdentityResource>()
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.Emphasize, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
                .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ConstructUsing(src => new IdentityResource(src.Name, src.Claims.Select(x => x.Name)));

        }
    }
}