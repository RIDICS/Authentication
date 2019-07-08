using System;
using AutoMapper;

namespace Ridics.Authentication.Service.MapperProfiles.Converters
{
    public class DateTimeOffsetToUtcDateTimeConverter : ITypeConverter<DateTimeOffset?, DateTime?>
    {
        public DateTime? Convert(DateTimeOffset? source, DateTime? destination, ResolutionContext context)
        {
            return source?.UtcDateTime;
        }
    }

    public class UtcDateTimeToDateTimeOffsetConverter : ITypeConverter<DateTime?, DateTimeOffset?>
    {
        public DateTimeOffset? Convert(DateTime? source, DateTimeOffset? destination, ResolutionContext context)
        {
            if(!source.HasValue) return null;

            //First specifiy that source is UTC
            var utc = DateTime.SpecifyKind(source.Value, DateTimeKind.Utc);

            //Second create offset
            DateTimeOffset offset = utc;

            return offset;
        }
    }
}