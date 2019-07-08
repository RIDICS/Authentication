using System;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Service.Models.ViewModel.Keys;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ApiAccessKeyProfile : Profile
    {
        public ApiAccessKeyProfile()
        {
            CreateMap<ApiAccessKeyModel, ApiAccessKeyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ForMember(dest => dest.ApiAccessKeyHashViewModel, opt => opt.MapFrom(src => src))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

            CreateMap<ApiAccessKeyModel, EditableApiAccessKeyViewModel>()
                .IncludeBase<ApiAccessKeyModel, ApiAccessKeyViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.EditableApiAccessKeyHashViewModel, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.HashAlgorithm, opt => opt.MapFrom(src => src.EditableApiAccessKeyHashViewModel.Algorithm))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

            CreateMap<ApiAccessKeyModel, ApiAccessKeyHashViewModel>()
                .ForMember(dest => dest.Algorithm, opt => opt.MapFrom(src => src.HashAlgorithm));

            CreateMap<ApiAccessKeyModel, EditableApiAccessKeyHashViewModel>()
                .IncludeBase<ApiAccessKeyModel, ApiAccessKeyHashViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));


            CreateMap<ApiAccessPermissionEnumViewModel, ApiAccessKeyPermissionModel>()
                .ForMember(dest => dest.Permission, opt => opt.MapFrom(src => Convert.ToInt32(src)));

            CreateMap<ApiAccessKeyPermissionModel, ApiAccessPermissionEnumViewModel>().ConvertUsing(src => (ApiAccessPermissionEnumViewModel)src.Permission);

            CreateMap<ApiAccessPermissionEnumViewModel, ApiAccessPermissionEnumModel>().ReverseMap();
        }
    }
}