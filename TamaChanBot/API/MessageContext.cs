using System;
using Discord;
using Discord.WebSocket;

namespace TamaChanBot.API
{
    public class MessageContext
    {
        protected readonly string message;
        protected readonly ulong authorId;
        protected readonly SocketUser author;
        protected readonly bool sentByBot;

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;
            this.authorId = message.Author.Id;
            this.author = message.Author;
            this.sentByBot = message.Author.IsBot;
        }
    }
}
