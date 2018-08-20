using System;
using System.Threading.Tasks;

namespace TamaChanBot.API.Events
{
    public interface IConnectionStatusReceiver
    {
        Task OnConnected();
        Task OnDisconnected(Exception exception);
    }
}
