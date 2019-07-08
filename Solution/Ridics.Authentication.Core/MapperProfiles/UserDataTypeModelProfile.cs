using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class UserDataTypeModelProfile : Profile
    {
        public UserDataTypeModelProfile()
        {
            CreateMap<UserDataTypeEntity, UserDataTypeModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DataTypeValue, opt => opt.MapFrom(src => src.DataTypeValue));
        }
    }
}