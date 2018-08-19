#pragma warning disable CS0618 //Disable Obsolete warning.
using System;
using System.Linq;
using System.Collections.Generic;
using Discord;
using Discord.WebSocket;

namespace TamaChanBot.API
{
    //The idea here is to lessen the support on the Discord.NET library in commands.
    //In doing so, updating the Discord.NET library won't break all the commands and will instead only
    //require me to update the MessageContext class.
    public class MessageContext
    {
        public readonly string message;
        public readonly string parameterString;
        public readonly ulong authorId;
        public readonly bool sentByBot;
        public readonly ulong? channelId;
        public readonly ulong? serverId;
        public readonly ulong messageId;
        public readonly bool isNSFWChannel;

        public IReadOnlyCollection<ulong> mentionedUserIds;
        public IReadOnlyCollection<ulong> mentionedRoleIds;
        public IReadOnlyCollection<ulong> mentionedChannelIds;

        [Obsolete("Usage not recommended.")]
        public readonly SocketUserMessage wrappedMessage;
        
        public bool IsInServer { get => serverId != null; }

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;

            int parameterStart = this.message.IndexOf(' ');
            if (parameterStart > 0)
                this.parameterString = this.message.Substring(parameterStart + 1);

            this.authorId = message.Author.Id;
            this.sentByBot = message.Author.IsBot;
            if(message.Channel is SocketGuildChannel)
            {
                this.channelId = (message.Channel as SocketGuildChannel).Id;
                this.serverId = (message.Channel as SocketGuildChannel).Guild.Id;
                if (message.Channel is ITextChannel)
                    this.isNSFWChannel = (message.Channel as ITextChannel).IsNsfw;
                else if (message.Channel is IDMChannel)
                    this.isNSFWChannel = true;
            }

            this.mentionedUserIds = message.MentionedUsers.Select(u => u.Id).ToList().AsReadOnly();
            this.mentionedRoleIds = message.MentionedRoles.Select(r => r.Id).ToList().AsReadOnly();
            this.mentionedChannelIds = message.MentionedChannels.Select(c => c.Id).ToList().AsReadOnly();

            this.messageId = message.Id;

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
