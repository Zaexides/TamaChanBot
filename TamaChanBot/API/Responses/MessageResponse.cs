using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace TamaChanBot.API.Responses
{
    public class MessageResponse : Response
    {
        public readonly string content;

        public MessageResponse(string content)
        {
            this.content = content;
        }

        internal override async Task Respond(ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync(content);
        }

        public override string ToString() => content;
    }
}
