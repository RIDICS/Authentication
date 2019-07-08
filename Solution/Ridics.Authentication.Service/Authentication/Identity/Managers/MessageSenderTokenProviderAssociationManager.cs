using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Core.MessageSenders;
using Ridics.Authentication.Service.Authentication.Identity.TokenProviders;


namespace Ridics.Authentication.Service.Authentication.Identity.Managers
{
    public class MessageSenderTokenProviderAssociationManager
    {
        private readonly Dictionary<string, IList<MessageSenderType>> m_senderAssociationDict;

        public MessageSenderTokenProviderAssociationManager()
        {
            m_senderAssociationDict = new Dictionary<string, IList<MessageSenderType>>
            {
                {TokenOptions.DefaultProvider, new List<MessageSenderType> {MessageSenderType.Email}}, //HACK make configurable according to IoC Identity registration
                {EmailTokenProvider.ProviderName, new List<MessageSenderType> {MessageSenderType.Email}},
                {SmsTokenProvider.ProviderName, new List<MessageSenderType> {MessageSenderType.SMS}}
            };
        }

        public IList<MessageSenderType> GetSenders(string provider)
        {
            if (m_senderAssociationDict.TryGetValue(provider, out var messageSenderTypes))
            {
                return messageSenderTypes;
            }

            return new List<MessageSenderType>(); //TODO log warning or throw exception
        }
    }
}