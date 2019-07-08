using System;
using System.Data;

namespace Ridics.Core.Shared.Exceptions
{
    public class DatabaseException : DataException
    {
        public DatabaseException()
        {
        }

        public DatabaseException(string message) : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
