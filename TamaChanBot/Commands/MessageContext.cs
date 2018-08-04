using System;
using Discord;
using Discord.WebSocket;

namespace TamaChanBot.Commands
{
    public class MessageContext
    {
        protected readonly string message;
        protected readonly SocketUser author;
        protected readonly bool sentByBot;

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;
            this.author = message.Author;
            this.sentByBot = message.Author.IsBot;
        }
    }
}
