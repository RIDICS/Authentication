using System;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Authentication.DataEntities.Exceptions
{
    public class SaveEntityException<T> : DatabaseException
    {
        public SaveEntityException() : base("Could not create entity", null)
        {
        }

        public SaveEntityException(string message) : base(message)
        {
        }

        public SaveEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class SaveContactEntityException : SaveEntityException<UserContactEntity>
    {
        public SaveContactEntityException() : base("Could not create entity", null)
        {
        }

        public SaveContactEntityException(string message) : base(message)
        {
        }

        public SaveContactEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ContactTypeEnum ContactType { get; set; }
    }
}