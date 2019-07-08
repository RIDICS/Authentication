using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.MessageSenders
{
    public abstract class SmsSenderBase : IMessageSender
    {
        public Task SendMessageAsync(UserModel user, string subject, string message)
        {
            var phoneNumber = user.UserContacts.Single(x => x.Type == ContactTypeEnumModel.Phone).Value;
            return SendSmsAsync(phoneNumber, message);
        }

        public abstract Task SendSmsAsync(string phoneNumber, string message);

        public MessageSenderType MessageSenderType => MessageSenderType.SMS;
    }

    public class NullSmsSender : SmsSenderBase
    {
        private readonly ILogger<NullSmsSender> m_logger;

        public NullSmsSender(ILogger<NullSmsSender> logger)
        {
            m_logger = logger;
        }

        public override Task SendSmsAsync(string phoneNumber, string message)
        {
            if (m_logger.IsEnabled(LogLevel.Warning))
                m_logger.LogWarning($"Can't send SMS to {phoneNumber} because NullSmsSender is configured instead of working one.");

            return Task.CompletedTask;
        }
    }
}