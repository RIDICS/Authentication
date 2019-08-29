using System;

namespace Ridics.Authentication.Core.MessageSenders
{
    public class MessageSenderException : Exception
    {
        public MessageSenderException()
        {
        }

        public MessageSenderException(string message) : base(message)
        {
        }

        public MessageSenderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}