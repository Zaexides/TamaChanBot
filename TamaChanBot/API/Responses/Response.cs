using System;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TamaChanBot.API.Responses
{
    public abstract class Response
    {
        internal abstract Task Respond(ISocketMessageChannel channel);

        public abstract override string ToString();
    }
}
