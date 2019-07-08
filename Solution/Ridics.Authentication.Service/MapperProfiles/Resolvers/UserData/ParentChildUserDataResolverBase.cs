using System.Linq;
using AutoMapper;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserData
{
    public class ParentChildUserDataResolverBase<T> : IValueResolver<UserModel, T, string>
    {
        protected virtual string ParentDataType { get; set; }

        protected virtual string ChildDataType { get; set; }

        public string Resolve(UserModel source, T destination, string destMember, ResolutionContext context)
        {
            var parentData = source.UserData.FirstOrDefault(x =>
                x.UserDataType.DataTypeValue == ParentDataType && x.ChildrenUserData != null && x.ChildrenUserData.Any());

            var childData = parentData?.ChildrenUserData.FirstOrDefault(y => y.UserDataType.DataTypeValue == ChildDataType);

            return childData?.Value;
        }
    }
}