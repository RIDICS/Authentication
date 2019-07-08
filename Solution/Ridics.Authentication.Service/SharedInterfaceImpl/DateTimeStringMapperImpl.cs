using System;
using Ridics.Authentication.Shared;
using Ridics.Core.Utils.Helpers;

namespace Ridics.Authentication.Service.SharedInterfaceImpl
{
    public class DateTimeStringMapperImpl : IDateTimeStringMapper
    {
        public string DateToString(DateTime date)
        {
            return DateTimeStringMapper.DateToString(date);
        }

        public DateTime StringToDate(string dateString)
        {
            return DateTimeStringMapper.StringToDate(dateString);
        }
    }
}