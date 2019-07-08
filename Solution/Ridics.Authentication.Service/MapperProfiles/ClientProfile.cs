using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer4.Models;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile(ISecretSorter secretSorter, IGrantTypeSorter grantTypeSorter, IUriSorter uriSorter,
            IApiResourceSorter apiResourceSorter, IIdentityResourceSorter identityResourceSorter)
        {
            CreateMap<ClientModel, ClientViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Secrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(dest => dest.AllowedGrantTypes, opt => opt.MapFrom(src => src.AllowedGrantTypes))
                .ForMember(dest => dest.UriList, opt => opt.MapFrom(src => src.UriList))
                .ForMember(dest => dest.AllowedIdentityResources, opt => opt.MapFrom(src => src.AllowedIdentityResources))
                .ForMember(dest => dest.DisplayUrl, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .AfterMap((src, dest) =>
                {
                    dest.Secrets = secretSorter.SortSecrets(dest.Secrets);
                    dest.AllowedGrantTypes = grantTypeSorter.SortGrantTypes(dest.AllowedGrantTypes);
                    dest.UriList = uriSorter.SortUris(dest.UriList);
                    dest.AllowedIdentityResources = identityResourceSorter.SortIdentityResources(dest.AllowedIdentityResources);
                })
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Secrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(dest => dest.AllowedGrantTypes, opt => opt.MapFrom(src => src.AllowedGrantTypes))
                .ForMember(dest => dest.UriList, opt => opt.MapFrom(src => src.UriList))
                .ForMember(dest => dest.AllowedIdentityResources, opt => opt.MapFrom(src => src.AllowedIdentityResources))
                .ForMember(dest => dest.DisplayUrl, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .ForMember(dest => dest.AllowAccessTokensViaBrowser, opt => opt.MapFrom(src => src.AllowAccessTokensViaBrowser))
                .ForMember(dest => dest.AllowedScopes, opt => opt.MapFrom(src => src.AllowedScopes))
                .AfterMap((src, dest) =>
                {
                    dest.Secrets = secretSorter.SortSecrets(dest.Secrets);
                    dest.AllowedGrantTypes = grantTypeSorter.SortGrantTypes(dest.AllowedGrantTypes);
                    dest.UriList = uriSorter.SortUris(dest.UriList);
                    dest.AllowedIdentityResources = identityResourceSorter.SortIdentityResources(dest.AllowedIdentityResources);
                });

            CreateMap<ClientViewModel, Client>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(dest => dest.AllowedGrantTypes, opt => opt.MapFrom(src => src.AllowedGrantTypes))
                .ForMember(dest => dest.RedirectUris, opt => opt.MapFrom(src => src.UriList.Where(x => x.IsRedirect).ToList()))
                .ForMember(
                    dest => dest.PostLogoutRedirectUris,
                    opt => opt.MapFrom(src => src.UriList.Where(x => x.IsPostLogoutRedirect).ToList()))
                .ForMember(dest => dest.AllowedCorsOrigins, opt => opt.MapFrom(src => src.UriList.Where(x => x.IsCorsOrigin).ToList())
                )
                .ForMember(dest => dest.ClientUri, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUri, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .ForMember(dest => dest.ClientSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(
                    dest => dest.FrontChannelLogoutUri,
                    opt => opt.MapFrom(src => src.UriList.FirstOrDefault(x => x.IsFrontChannelLogout))
                )
                .ForMember(dest => dest.AllowAccessTokensViaBrowser, opt => opt.MapFrom(src => src.AllowAccessTokensViaBrowser))
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.ProtocolType, opt => opt.Ignore())
                .ForMember(dest => dest.RequireClientSecret, opt => opt.Ignore())
                .ForMember(dest => dest.AllowRememberConsent, opt => opt.Ignore())
                .ForMember(dest => dest.RequirePkce, opt => opt.Ignore())
                .ForMember(dest => dest.AllowPlainTextPkce, opt => opt.Ignore())
                .ForMember(dest => dest.FrontChannelLogoutSessionRequired, opt => opt.Ignore())
                .ForMember(dest => dest.BackChannelLogoutUri, opt => opt.Ignore())
                .ForMember(dest => dest.BackChannelLogoutSessionRequired, opt => opt.Ignore())
                .ForMember(dest => dest.AllowOfflineAccess, opt => opt.Ignore())
                .ForMember(dest => dest.AlwaysIncludeUserClaimsInIdToken, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AccessTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationCodeLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AbsoluteRefreshTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.SlidingRefreshTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.ConsentLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenUsage, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAccessTokenClaimsOnRefresh, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiration, opt => opt.Ignore())
                .ForMember(dest => dest.AccessTokenType, opt => opt.Ignore())
                .ForMember(dest => dest.EnableLocalLogin, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(dest => dest.IncludeJwtId, opt => opt.Ignore())
                .ForMember(dest => dest.Claims, opt => opt.Ignore())
                .ForMember(dest => dest.AlwaysSendClientClaims, opt => opt.Ignore())
                .ForMember(dest => dest.ClientClaimsPrefix, opt => opt.Ignore())
                .ForMember(dest => dest.PairWiseSubjectSalt, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedScopes, opt => opt.Ignore())
                .ForMember(dest => dest.UserSsoLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.UserCodeType, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceCodeLifetime, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var allowedScopes = new List<string>();
                    allowedScopes.AddRange(src.AllowedIdentityResources.Select(x => x.Name));
                    allowedScopes.AddRange(src.AllowedScopes.Select(x => x.Name));

                    dest.AllowedScopes = allowedScopes;
                });

            CreateMap<ClientModel, Client>()
                .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ClientSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(dest => dest.AllowedGrantTypes, opt => opt.MapFrom(src => src.AllowedGrantTypes))
                .ForMember(
                    dest => dest.RedirectUris,
                    opt => opt.MapFrom(src => src.UriList.Where(x => x.IsUriOfType(UriModelHelper.Redirect)).ToList())
                )
                .ForMember(dest => dest.PostLogoutRedirectUris,
                    opt => opt.MapFrom(src => src.UriList.Where(x => x.IsUriOfType(UriModelHelper.PostLougoutRedirect)).ToList()))
                .ForMember(
                    dest => dest.AllowedCorsOrigins,
                    opt => opt.MapFrom(src => src.UriList.Where(x => x.IsUriOfType(UriModelHelper.CorsOrigin)).ToList())
                )
                .ForMember(dest => dest.ClientUri, opt => opt.MapFrom(src => src.DisplayUrl))
                .ForMember(dest => dest.LogoUri, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.RequireConsent, opt => opt.MapFrom(src => src.RequireConsent))
                .ForMember(dest => dest.ClientSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ForMember(
                    dest => dest.FrontChannelLogoutUri,
                    opt => opt.MapFrom(src => src.UriList.FirstOrDefault(x => x.IsUriOfType(UriModelHelper.FrontChannelLogout)))
                )
                .ForMember(dest => dest.AllowAccessTokensViaBrowser, opt => opt.MapFrom(src => src.AllowAccessTokensViaBrowser))
                .ForMember(dest => dest.Enabled, opt => opt.Ignore())
                .ForMember(dest => dest.ProtocolType, opt => opt.Ignore())
                .ForMember(dest => dest.RequireClientSecret, opt => opt.Ignore())
                .ForMember(dest => dest.AllowRememberConsent, opt => opt.Ignore())
                .ForMember(dest => dest.RequirePkce, opt => opt.Ignore())
                .ForMember(dest => dest.AllowPlainTextPkce, opt => opt.Ignore())
                .ForMember(dest => dest.FrontChannelLogoutSessionRequired, opt => opt.Ignore())
                .ForMember(dest => dest.BackChannelLogoutUri, opt => opt.Ignore())
                .ForMember(dest => dest.BackChannelLogoutSessionRequired, opt => opt.Ignore())
                .ForMember(dest => dest.AllowOfflineAccess, opt => opt.Ignore())
                .ForMember(dest => dest.AlwaysIncludeUserClaimsInIdToken, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AccessTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorizationCodeLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.AbsoluteRefreshTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.SlidingRefreshTokenLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.ConsentLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenUsage, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateAccessTokenClaimsOnRefresh, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokenExpiration, opt => opt.Ignore())
                .ForMember(dest => dest.AccessTokenType, opt => opt.Ignore())
                .ForMember(dest => dest.EnableLocalLogin, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityProviderRestrictions, opt => opt.Ignore())
                .ForMember(dest => dest.IncludeJwtId, opt => opt.Ignore())
                .ForMember(dest => dest.Claims, opt => opt.Ignore())
                .ForMember(dest => dest.AlwaysSendClientClaims, opt => opt.Ignore())
                .ForMember(dest => dest.ClientClaimsPrefix, opt => opt.Ignore())
                .ForMember(dest => dest.PairWiseSubjectSalt, opt => opt.Ignore())
                .ForMember(dest => dest.Properties, opt => opt.Ignore())
                .ForMember(dest => dest.AllowedScopes, opt => opt.Ignore())
                .ForMember(dest => dest.UserSsoLifetime, opt => opt.Ignore())
                .ForMember(dest => dest.UserCodeType, opt => opt.Ignore())
                .ForMember(dest => dest.DeviceCodeLifetime, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.AllowOfflineAccess = true;
                    dest.RefreshTokenUsage = TokenUsage.ReUse;
                    dest.UpdateAccessTokenClaimsOnRefresh = true;

                    var allowedScopes = new List<string>();
                    allowedScopes.AddRange(src.AllowedIdentityResources.Select(x => x.Name));
                    allowedScopes.AddRange(src.AllowedScopes.Select(x => x.Name));

                    dest.AllowedScopes = allowedScopes;
                });
        }
    }
}
