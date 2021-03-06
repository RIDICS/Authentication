﻿using System.Collections.Generic;
using AutoMapper;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.MapperProfiles.Resolvers.UserContacts
{
    public class UserContactToListResolver<T> : IValueResolver<T, UserModel, IList<UserContactModel>> where T : IConvertibleToUserContacts
    {
        public IList<UserContactModel> Resolve(T source, UserModel destination, IList<UserContactModel> destMember,
            ResolutionContext context)
        {
            var userContacts = new List<UserContactModel>();

            if (!string.IsNullOrEmpty(source.Email))
            {
                userContacts.Add(new UserContactModel
                {
                    User = destination,
                    LevelOfAssurance = new LevelOfAssuranceModel
                    {
                        Level = (int) source.EmailLevelOfAssurance,
                        Name = (LevelOfAssuranceEnumModel) source.EmailLevelOfAssurance,
                    },
                    Value = source.Email,
                    Type = ContactTypeEnumModel.Email,
                    ConfirmCode = source.EmailConfirmCode,
                });
            }

            if (!string.IsNullOrEmpty(source.PhoneNumber))
            {
                userContacts.Add(new UserContactModel
                {
                    User = destination,
                    LevelOfAssurance = new LevelOfAssuranceModel
                    {
                        Level = (int) source.PhoneLevelOfAssurance,
                        Name = (LevelOfAssuranceEnumModel) source.PhoneLevelOfAssurance,
                    },
                    Value = source.PhoneNumber,
                    Type = ContactTypeEnumModel.Phone,
                    ConfirmCode = source.PhoneNumberConfirmCode,
                });
            };

            return userContacts;
        }
    }
}