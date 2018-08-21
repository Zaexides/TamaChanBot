using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using TamaChanBot.Core;
using TamaChanBot.API.Events;
using Discord;
using Discord.WebSocket;
using Discord.Rest;

namespace TamaChanBot.API.Responses
{
    public class MenuResponse : EmbedResponse
    {
        private readonly ulong userId, channelId;
        private readonly Menu menu;

#pragma warning disable CS0618 //Disable Obsolete warning.
        public MenuResponse(Menu menu, MessageContext context)
        {
            this.Author = context.authorName;

            this.IconUrl = context.wrappedMessage.Author.GetAvatarUrl(ImageFormat.Auto, 64);
            if (context.wrappedMessage.Author is SocketGuildUser)
            {
                IReadOnlyCollection<SocketRole> roles = (context.wrappedMessage.Author as SocketGuildUser).Roles;
                if (roles.Count > 0)
                {
                    IEnumerator<SocketRole> enumerator = roles.GetEnumerator();
                    while(enumerator.MoveNext() && this.Color == 0)
                        this.Color = enumerator.Current.Color.RawValue;
                }
            }

            this.Title = menu.title;
            this.messages = new Message[1]
            {
                new Message("Options:", string.Join("\r\n", menu.options))
            };

            this.userId = context.authorId;
            this.channelId = context.wrappedMessage.Channel.Id;
            this.menu = menu;
        }
#pragma warning restore CS0618 //Enable Obsolete warning.

        internal override async Task<ResponseSentArgs> Respond(ISocketMessageChannel channel)
        {
            TamaChan.Instance.EventSystem.MenuHandler.AddOrReplaceActiveMenu(this.userId, this.channelId, this.menu);
            return await base.Respond(channel);
        }
    }
}
