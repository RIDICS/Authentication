using System;
using System.Threading.Tasks;
using MailKit;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Ridics.Authentication.Core.MessageSenders;
using Ridics.Authentication.Service.Configuration;

namespace Ridics.Authentication.Service.Utils.MessageSenders
{
    public class SmtpEmailSender : EmailSenderBase
    {
        private readonly SmtpEmailConfiguration m_emailConfiguration;
        private readonly ILogger m_logger;

        public SmtpEmailSender(SmtpEmailConfiguration emailConfiguration, ILogger logger)
        {
            m_emailConfiguration = emailConfiguration;
            m_logger = logger;
        }

        public override Task SendEmailAsync(string emailAddress, string subject, string messageContent)
        {
            return Task.Run(() => SendEmail(emailAddress, subject, messageContent));
        }

        private void SendEmail(string emailAddress, string subject, string messageContent)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(emailAddress));
            message.From.Add(new MailboxAddress(m_emailConfiguration.SenderAddress));
            
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = messageContent
            };

            try
            {
                using (var emailClient = new DsnSmtpClient())
                {
                    //The last parameter here is to use SSL
                    emailClient.Connect(m_emailConfiguration.SmtpServer, m_emailConfiguration.SmtpPort);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    if (!string.IsNullOrEmpty(m_emailConfiguration.SmtpUsername) ||
                        !string.IsNullOrEmpty(m_emailConfiguration.SmtpPassword))
                    {
                        emailClient.Authenticate(m_emailConfiguration.SmtpUsername, m_emailConfiguration.SmtpPassword);
                    }

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
            }
            catch (CommandException e)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(e, "The send command failed.");
                }

                throw new MessageSenderException("The send command failed.", e);
            }
            catch (ProtocolException e)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(e, "A protocol exception occurred.");
                }

                throw new MessageSenderException("A protocol exception occurred.", e);
            }
            catch (OperationCanceledException e)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(e, "The operation has been canceled.");
                }

                throw new MessageSenderException("The operation has been canceled.", e);
            }
            catch (ServiceNotAuthenticatedException e)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(e, "Authentication is required before sending a message.");
                }

                throw new MessageSenderException("Authentication is required before sending a message.", e);
            }
            catch (InvalidOperationException e)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(e, "A sender has not been specified or no recipients have been specified.");
                }

                throw new MessageSenderException("A sender has not been specified or no recipients have been specified.", e);
            }
        }
    }
}
