using System;

namespace Ridics.Authentication.Service.Exceptions
{
    public class StoreException : Exception
    {
        public StoreException()
        {
        }

        public StoreException(string message) : base(message)
        {
        }

        public StoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}