using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class ResourcePermissionTypeProfile : Profile
    {
        public ResourcePermissionTypeProfile(IResourcePermissionTypeActionSorter resourcePermissionTypeActionSorter)
        {
            CreateMap<ResourcePermissionTypeInfoModel, ResourcePermissionTypeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ResourcePermissionTypeViewModel, ResourcePermissionTypeInfoModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<ResourcePermissionTypeModel, ResourcePermissionTypeViewModel>()
                .IncludeBase<ResourcePermissionTypeInfoModel, ResourcePermissionTypeViewModel>()
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                }); 

            CreateMap<ResourcePermissionTypeViewModel, ResourcePermissionTypeModel>()
                .IncludeBase<ResourcePermissionTypeViewModel, ResourcePermissionTypeInfoModel>()
                .ForMember(dest => dest.ResourcePermissionTypeActions, opt => opt.MapFrom(src => src.ResourcePermissionTypeActions))
                .AfterMap((src, dest) =>
                {
                    dest.ResourcePermissionTypeActions = resourcePermissionTypeActionSorter.SortResourcePermissionTypeActions(dest.ResourcePermissionTypeActions);
                });
        }
    }
}