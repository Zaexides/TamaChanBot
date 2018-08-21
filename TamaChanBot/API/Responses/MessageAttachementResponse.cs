using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Rest;
using TamaChanBot.API.Events;

namespace TamaChanBot.API.Responses
{
    public class MessageAttachementResponse : MessageResponse
    {
        public readonly string filePath;

        public MessageAttachementResponse(string message, string filePath) : base(message)
        {
            this.filePath = filePath;
        }

        internal override async Task<ResponseSentArgs> Respond(ISocketMessageChannel channel)
        {
            RestUserMessage msg = await channel.SendFileAsync(filePath, content);
            return new ResponseSentArgs(msg);
        }

        public override string ToString()
        {
            return base.ToString() + $"(Attachement: \"{filePath}\")";
        }
    }
}
