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
    public abstract class BaseMenuResponse : EmbedResponse
    {
        protected readonly ulong userId, channelId;
        protected readonly IMenuHandlerObject menuHandlerObject;

#pragma warning disable CS0618 //Disable Obsolete warning.
        public BaseMenuResponse(IMenuHandlerObject menuHandlerObject, MessageContext context)
        {
            this.Author = context.authorName;

            this.IconUrl = context.authorAvatarUrl;
            if (context.wrappedMessage.Author is SocketGuildUser)
            {
                IReadOnlyCollection<SocketRole> roles = (context.wrappedMessage.Author as SocketGuildUser).Roles;
                if (roles.Count > 0)
                {
                    IEnumerator<SocketRole> enumerator = roles.GetEnumerator();
                    while (enumerator.MoveNext() && this.Color == 0)
                        this.Color = enumerator.Current.Color.RawValue;
                }
            }

            this.Title = menuHandlerObject.Title;

            this.Description = menuHandlerObject.Description;
            this.Footer = menuHandlerObject.Footer;

            this.userId = context.authorId;
            this.channelId = context.wrappedMessage.Channel.Id;
            this.menuHandlerObject = menuHandlerObject;
        }
#pragma warning restore CS0618 //Enable Obsolete warning.

        internal override async Task<ResponseSentArgs> Respond(ISocketMessageChannel channel)
        {
            TamaChan.Instance.EventSystem.MenuHandler.AddOrReplaceActiveMenu(this.userId, this.channelId, this.menuHandlerObject);
            ResponseSentArgs responseSentArgs = await base.Respond(channel);
            menuHandlerObject.ResponseSentArgs = responseSentArgs;
            return responseSentArgs;
        }
    }
}
