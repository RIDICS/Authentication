using System;

namespace Ridics.Authentication.DataEntities.Exceptions
{
    public class UserDataAlreadyExistsException : ArgumentException
    {
        private static readonly string m_messageTemplate = "User data of type '{1}' with value '{0}' already exists.";

        public UserDataAlreadyExistsException(string value, string type) : base(string.Format(m_messageTemplate, value, type))
        {
            DataType = type;
        }

        public UserDataAlreadyExistsException(string value, string type, Exception ex) : base(string.Format(m_messageTemplate, value, type), ex)
        {
            DataType = type;
        }

        public string DataType { get; }
    }
}