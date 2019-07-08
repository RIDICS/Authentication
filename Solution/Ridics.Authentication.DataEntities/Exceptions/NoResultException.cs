using System;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Exceptions
{
    public class NoResultException<T> : DatabaseException
    {
        public NoResultException() : base("Could not find any matching result", null)
        {
        }

        public NoResultException(string message) : base(message)
        {
        }

        public NoResultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}