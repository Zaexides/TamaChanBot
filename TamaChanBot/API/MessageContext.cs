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
        public readonly string authorName;
        public readonly string authorAvatarUrl;
        public readonly ulong authorId;
        public readonly bool sentByBot;
        public readonly ulong? channelId;
        public readonly ulong? serverId;
        public readonly ulong messageId;
        public readonly bool isNSFWChannel;
        public readonly Permission authorPermissions;

        public MentionInfo[] mentionedUsers;
        public MentionInfo[] mentionedRoles;
        public MentionInfo[] mentionedChannels;

        [Obsolete("Usage not recommended.")]
        public readonly SocketUserMessage wrappedMessage;
        
        public bool IsInServer { get => serverId != null; }

        public MessageContext(SocketUserMessage message)
        {
            this.message = message.Content;

            int parameterStart = this.message.IndexOf(' ');
            if (parameterStart > 0)
                this.parameterString = this.message.Substring(parameterStart + 1);

            this.authorName = message.Author.Username;
            this.authorAvatarUrl = message.Author.GetAvatarUrl(ImageFormat.Auto, 64);

            this.authorId = message.Author.Id;
            this.sentByBot = message.Author.IsBot;
            if(message.Channel is SocketGuildChannel)
            {
                this.channelId = (message.Channel as SocketGuildChannel).Id;
                this.serverId = (message.Channel as SocketGuildChannel).Guild.Id;
                if (message.Channel is ITextChannel)
                    this.isNSFWChannel = (message.Channel as ITextChannel).IsNsfw;

                SocketGuildUser authorGuildUser = message.Author as SocketGuildUser;
                if (authorGuildUser.Nickname != null && !authorGuildUser.Nickname.Equals(string.Empty))
                    this.authorName = authorGuildUser.Nickname;
                this.authorPermissions = (Permission)authorGuildUser.GetPermissions((message.Channel as IGuildChannel)).RawValue;
            }

            if (message.Channel is IDMChannel)
            {
                this.isNSFWChannel = true;
                this.authorPermissions = (Permission)ulong.MaxValue;
            }

            List<MentionInfo> mentions = new List<MentionInfo>();
            foreach (SocketUser user in message.MentionedUsers)
            {
                string name = user.Username;
                if(user is SocketGuildUser)
                {
                    SocketGuildUser guildUser = user as SocketGuildUser;
                    if (guildUser.Nickname != null && guildUser.Nickname != string.Empty)
                        name = guildUser.Nickname;
                }
                mentions.Add(new MentionInfo(user.Id, name, "@"));
            }
            mentionedUsers = mentions.ToArray();

            mentions.Clear();
            foreach (SocketRole role in message.MentionedRoles)
                mentions.Add(new MentionInfo(role.Id, role.Name, "@&"));
            mentionedRoles = mentions.ToArray();

            foreach (SocketChannel channel in message.MentionedChannels)
            {
                string name = string.Empty;
                if (channel is SocketGuildChannel)
                    name = (channel as SocketGuildChannel).Name;
                mentions.Add(new MentionInfo(channel.Id, name, "#"));
            }
            mentionedChannels = mentions.ToArray();

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

        public class MentionInfo
        {
            public readonly ulong id;
            public readonly string name;
            public readonly string prefix;

            internal MentionInfo(ulong id, string name, string prefix)
            {
                this.id = id;
                this.name = name;
                this.prefix = prefix;
            }

            public static implicit operator ulong(MentionInfo mentionInfo) => mentionInfo.id;

            public override string ToString()
            {
                return $"<{prefix}{id}>";
            }
        }
    }
}
