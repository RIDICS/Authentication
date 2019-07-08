using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class UserContactContractProfile : Profile
    {
        public UserContactContractProfile()
        {
            CreateMap<UserContactBasicInfoModel, UserContactContract>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ContactType, opt => opt.MapFrom(src => src.ContactType))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
        }
    }
}