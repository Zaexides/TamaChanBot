using System;
using System.Threading.Tasks;

namespace TamaChanBot.API.Events
{
    public interface IMessageReceiver
    {
        Task OnMessageReceived(MessageReceivedArgs messageReceivedArgs);
    }
}
