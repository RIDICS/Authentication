using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.MessageSenders;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public class MessageSenderManager : ManagerBase
    {
        private readonly Dictionary<MessageSenderType, IMessageSender> m_messageSendersDict;

        public MessageSenderManager(IEnumerable<IMessageSender> messageSenders, ILogger logger, ITranslator translator, IMapper mapper,
            IPaginationConfiguration paginationConfiguration) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_messageSendersDict = messageSenders.ToDictionary(x => x.MessageSenderType);
        }

        public void SendMessage(UserModel user, IEnumerable<MessageSenderType> messageSenderTypes, string subject, string message)
        {
            foreach (var messageSenderType in messageSenderTypes)
            {
                SendMessage(user, messageSenderType, subject, message);
            }
        }

        public void SendMessage(UserModel user, MessageSenderType messageSenderType, string subject, string message)
        {
            m_messageSendersDict[messageSenderType].SendMessageAsync(user, subject, message);
        }
    }
}