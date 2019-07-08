using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class FileResourceModelProfile : Profile
    {
        public FileResourceModelProfile()
        {
            CreateMap<FileResourceEntity, FileResourceModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.FileExtension, opt => opt.MapFrom(src => src.FileExtension));
        }
    }
}