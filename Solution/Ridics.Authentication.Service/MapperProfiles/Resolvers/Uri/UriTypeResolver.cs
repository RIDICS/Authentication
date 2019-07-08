using System.Collections.Generic;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.Uri
{
    public class UriTypeViewModelResolver<T> : IValueResolver<UriModel, T, bool>
    {
        private readonly string m_uriType;

        public UriTypeViewModelResolver(string uriType)
        {
            m_uriType = uriType;
        }

        public bool Resolve(UriModel source, T destination, bool destMember, ResolutionContext context)
        {
           return source.IsUriOfType(m_uriType);
        }
    }

    public class UriTypeModelResolver<T> : IValueResolver<UriViewModel, T, IList<UriTypeModel>>
    {
        public IList<UriTypeModel> Resolve(UriViewModel src, T destination, IList<UriTypeModel> destMember, ResolutionContext context)
        {
            var uriTypes = new List<UriTypeModel>();

            if (src.IsRedirect)
            {
                uriTypes.Add(new UriTypeModel { UriTypeValue = UriModelHelper.Redirect });
            }

            if (src.IsPostLogoutRedirect)
            {
                uriTypes.Add(new UriTypeModel { UriTypeValue = UriModelHelper.PostLougoutRedirect });
            }

            if (src.IsCorsOrigin)
            {
                uriTypes.Add(new UriTypeModel { UriTypeValue = UriModelHelper.CorsOrigin });
            }

            if (src.IsFrontChannelLogout)
            {
                uriTypes.Add(new UriTypeModel { UriTypeValue = UriModelHelper.FrontChannelLogout });
            }

            return uriTypes;
        }
    }
}