using System;

namespace Ridics.Authentication.DataEntities.Exceptions
{
    public class VerficationCodeAlreadyExistsException : ArgumentException
    {
        private static readonly string m_messageTemplate = "Verification code '{0}' already exists.";

        public VerficationCodeAlreadyExistsException(string code) : base(string.Format(m_messageTemplate, code))
        {
        }

        public VerficationCodeAlreadyExistsException(string code, Exception ex) : base(string.Format(m_messageTemplate, code), ex)
        {
        }
    }
}