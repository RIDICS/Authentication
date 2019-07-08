using System;
using Ridics.Authentication.Core.Models.DataResult;

namespace Ridics.Authentication.Service.Exceptions
{
    public class ConfirmContactException : Exception
    {
        public DataResultError Error { get; set; }
    }
}