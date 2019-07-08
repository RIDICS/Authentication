using System;

namespace Ridics.Authentication.Service.Exceptions
{
    public class GenerateResetPasswordTokenException : Exception
    {
        private static readonly string m_messageTemplate = "Can not generate reset password token.";

        public GenerateResetPasswordTokenException() : base(m_messageTemplate)
        {
        }

        public GenerateResetPasswordTokenException(Exception ex) : base(m_messageTemplate, ex)
        {
        }
    }
}