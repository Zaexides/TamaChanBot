using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using TamaChanBot.API;
using TamaChanBot.API.Responses;

namespace TamaChanBot.API.Events
{
    public class ResponseSentArgs
    {
        public readonly ulong id;
        public readonly ulong channelId;
        public readonly ulong? serverId;

        internal RestUserMessage message;

        internal ResponseSentArgs(RestUserMessage restUserMessage)
        {
            this.id = restUserMessage.Id;
            this.channelId = restUserMessage.Channel.Id;
            if (restUserMessage.Channel is IGuildChannel)
                this.serverId = (restUserMessage.Channel as IGuildChannel).Guild.Id;
            else
                this.serverId = null;
            this.message = restUserMessage;
        }

        internal ResponseSentArgs(MessageContext context)
        {
            this.id = context.messageId;
            this.channelId = context.channelId == null ? 0 : (ulong)context.channelId;
            this.serverId = context.serverId;
            this.message = null;
        }

        public void Delete() => Task.Run(() => DeleteAsync()).Wait();
        public void Modify(string newMessage) => Task.Run(() => ModifyAsync(newMessage)).Wait();
        public void Modify(EmbedResponse embed) => Task.Run(() => ModifyAsync(embed)).Wait();

        private async Task DeleteAsync()
        {
            await message.DeleteAsync();
        }

        private async Task ModifyAsync(string newMessage)
        {
            await message.ModifyAsync((mp) => mp.Content = newMessage);
        }

        private async Task ModifyAsync(EmbedResponse embedResponse)
        {
            await message.ModifyAsync((mp) => mp.Embed = embedResponse.CreateDiscordEmbed());
        }
    }
}
