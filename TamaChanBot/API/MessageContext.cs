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
        protected readonly string message;
        protected readonly ulong authorId;
        protected readonly bool sentByBot;

        [Obsolete("Usage not recommended.")]
        protected readonly SocketUserMessage wrappedMessage;

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;
            this.authorId = message.Author.Id;
            this.sentByBot = message.Author.IsBot;

            this.wrappedMessage = message;
        }
    }
}
