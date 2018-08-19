using System;
using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot.API.Events
{
    public interface IMessageReceiver
    {
        Task OnMessageReceived(MessageReceivedArgs messageReceivedArgs);
    }
}
