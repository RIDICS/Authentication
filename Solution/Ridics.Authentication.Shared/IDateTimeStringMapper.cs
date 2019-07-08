using System;

namespace Ridics.Authentication.Shared
{
    public interface IDateTimeStringMapper
    {
        string DateToString(DateTime date);

        DateTime StringToDate(string dateString);
    }
}