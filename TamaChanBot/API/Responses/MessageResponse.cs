using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Rest;
using TamaChanBot.API.Events;

namespace TamaChanBot.API.Responses
{
    public class MessageResponse : Response
    {
        public readonly string content;

        public MessageResponse(string content)
        {
            this.content = content;
        }

        internal override async Task<ResponseSentArgs> Respond(ISocketMessageChannel channel)
        {
            RestUserMessage msg = await channel.SendMessageAsync(content);
            return new ResponseSentArgs(msg);
        }

        public override string ToString() => content;
    }
}
