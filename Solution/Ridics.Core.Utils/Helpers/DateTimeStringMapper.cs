using System;
using System.Globalization;

namespace Ridics.Core.Utils.Helpers
{
    public static class DateTimeStringMapper
    {
        private const string Format = "yyyy-MM-dd";

        public static string DateToString(DateTime date)
        {
            return date.ToString(Format);
        }

        public static DateTime StringToDate(string dateString)
        {
            var result = DateTime.TryParseExact(dateString, Format, null, DateTimeStyles.None, out var date);

            if (result)
            {
                return date;
            }

            throw new ArgumentException($"Specified date string is not in format {Format}");
        }

    }
}