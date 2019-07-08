using System;

namespace Ridics.Authentication.Service.Exceptions
{
    public class UserDataException : Exception
    {
        public UserDataException()
        {
        }

        public UserDataException(string message) : base(message)
        {
        }
    }
}
