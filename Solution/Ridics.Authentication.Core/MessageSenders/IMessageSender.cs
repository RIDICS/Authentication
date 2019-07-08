using System.Threading.Tasks;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.MessageSenders
{
    public interface IMessageSender
    {
        Task SendMessageAsync(UserModel user, string subject, string message);

        MessageSenderType MessageSenderType { get; }
    }
}