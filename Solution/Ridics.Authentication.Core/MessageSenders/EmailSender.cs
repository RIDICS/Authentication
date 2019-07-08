using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.MessageSenders
{
    public abstract class EmailSenderBase : IMessageSender
    {
        public Task SendMessageAsync(UserModel user, string subject, string message)
        {
            var emailAddress = user.UserContacts.Single(x => x.Type == ContactTypeEnumModel.Email).Value;
            return SendEmailAsync(emailAddress, subject, message);
        }

        public abstract Task SendEmailAsync(string emailAddress, string subject, string message);

        public MessageSenderType MessageSenderType => MessageSenderType.Email;
    }

    public class NullEmailSender : EmailSenderBase
    {
        private readonly ILogger<NullEmailSender> m_logger;

        public NullEmailSender(ILogger<NullEmailSender> logger)
        {
            m_logger = logger;
        }

        public override Task SendEmailAsync(string emailAddress, string subject, string message)
        {
            if (m_logger.IsEnabled(LogLevel.Warning))
                m_logger.LogWarning($"Can't send e-mail to {emailAddress} because NullEmailSender is configured instead of working one.");
            return Task.CompletedTask;
        }
    }
}