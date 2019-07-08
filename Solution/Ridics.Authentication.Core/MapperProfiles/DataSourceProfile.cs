using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class DataSourceProfile : Profile
    {
        public DataSourceProfile()
        {
            CreateMap<DataSourceEntity, DataSourceModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource))
                .ForMember(dest => dest.ExternalLoginProvider, opt => opt.MapFrom(src => src.ExternalLoginProvider));
        }
    }
}
