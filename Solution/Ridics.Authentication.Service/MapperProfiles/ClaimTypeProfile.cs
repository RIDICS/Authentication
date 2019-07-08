using System.Security.Claims;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ClaimTypeProfile : Profile
    {
        public ClaimTypeProfile()
        {
            CreateMap<ClaimTypeModel, ClaimTypeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.SelectedType, opt => opt.MapFrom(src => src.SelectedType))
                .ForMember(dest => dest.AllTypeValues, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.SelectedType, opt => opt.MapFrom(src => src.SelectedType))
                .ForMember(dest => dest.AllTypeValues, opt => opt.Ignore());

            CreateMap<Claim, ClaimTypeModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedType, opt => opt.Ignore())
                .ForMember(dest => dest.AllTypeValues, opt => opt.Ignore());
        }
    }
}