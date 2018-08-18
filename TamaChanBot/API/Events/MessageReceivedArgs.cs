using System;
using Discord.WebSocket;

namespace TamaChanBot.API.Events
{
    public class MessageReceivedArgs : EventArgs
    {
        public readonly string content;
        public readonly ulong userId;
        public readonly ulong? channelId;
        public readonly ulong? serverId;
        public readonly bool isCommand;

        internal SocketUserMessage userMessage;

        internal MessageReceivedArgs(SocketUserMessage userMessage, bool isCommand)
        {
            this.userMessage = userMessage;

            this.content = userMessage.Content;
            this.userId = userMessage.Author.Id;

            if (userMessage.Channel is SocketGuildChannel)
            {
                this.channelId = userMessage.Channel.Id;
                this.serverId = (userMessage.Channel as SocketGuildChannel).Guild.Id;
            }

            this.isCommand = isCommand;
        }

        public MessageContext CreateMessageContext() => new MessageContext(this.userMessage);
    }
}
