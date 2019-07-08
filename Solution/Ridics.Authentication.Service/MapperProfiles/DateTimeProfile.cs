using System;
using AutoMapper;
using Ridics.Authentication.Service.MapperProfiles.Converters;

namespace Ridics.Authentication.Service.MapperProfiles
{
    public class DateTimeProfile : Profile
    {
        /// <summary>
        /// Creates map that is using custom converter to map from DateTimeOffset to UTCDateTime and vice versa
        /// </summary>
        public DateTimeProfile()
        {
            CreateMap<DateTimeOffset?, DateTime?>().ConvertUsing<DateTimeOffsetToUtcDateTimeConverter>();

            CreateMap<DateTime?, DateTimeOffset?>().ConvertUsing<UtcDateTimeToDateTimeOffsetConverter>();
        }
    }
}