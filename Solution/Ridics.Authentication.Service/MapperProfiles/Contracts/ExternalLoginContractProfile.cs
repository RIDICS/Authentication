using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts.User;

namespace Ridics.Authentication.Service.MapperProfiles.Contracts
{
    public class ExternalLoginContractProfile : Profile
    {
        public ExternalLoginContractProfile()
        {
            CreateMap<ExternalUserLoginModel, ExternalLoginContract>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LoginProviderId, opt => opt.MapFrom(src => src.LoginProvider.Id));
        }
    }
}