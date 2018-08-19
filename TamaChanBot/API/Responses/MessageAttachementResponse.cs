using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace TamaChanBot.API.Responses
{
    public class MessageAttachementResponse : MessageResponse
    {
        public readonly string filePath;

        public MessageAttachementResponse(string message, string filePath) : base(message)
        {
            this.filePath = filePath;
        }

        internal override async Task Respond(ISocketMessageChannel channel)
        {
            await channel.SendFileAsync(filePath, content);
        }

        public override string ToString()
        {
            return base.ToString() + $"(Attachement: \"{filePath}\")";
        }
    }
}
