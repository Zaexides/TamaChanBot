#pragma warning disable CS0618 //Disable Obsolete warning.
using System;
using Discord.WebSocket;

namespace TamaChanBot.API
{
    //The idea here is to lessen the support on the Discord.NET library in commands.
    //In doing so, updating the Discord.NET library won't break all the commands and will instead only
    //require me to update the MessageContext class.
    public class MessageContext
    {
        public readonly string message;
        public readonly ulong authorId;
        public readonly bool sentByBot;
        public readonly ulong? channelId;
        public readonly ulong? serverId;

        [Obsolete("Usage not recommended.")]
        protected readonly SocketUserMessage wrappedMessage;

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;
            this.authorId = message.Author.Id;
            this.sentByBot = message.Author.IsBot;
            if(message.Channel is SocketGuildChannel)
            {
                this.channelId = (message.Channel as SocketGuildChannel).Id;
                this.serverId = (message.Channel as SocketGuildChannel).Guild.Id;
            }
            this.wrappedMessage = message;
        }

        public override string ToString()
        {
            string str = $"{{{authorId}}} ";
            if (sentByBot)
                str += "[BOT] ";
            if (serverId != null)
                str += $"@{serverId}.{channelId} ";
            str += $"- \"{message}\"";
            return str;
        }
    }
}
