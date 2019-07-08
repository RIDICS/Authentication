using AutoMapper;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataContracts;

namespace Ridics.Authentication.Core.MapperProfiles
{
    public class ContactTypeProfile : Profile
    {
        public ContactTypeProfile()
        {
            CreateMap<ContactTypeEnum, ContactTypeEnumModel>().ReverseMap();
        }
    }
}
