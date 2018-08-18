using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

namespace TamaChanBot.API.Responses
{
    public class EmbedResponse : Response
    {
        public string Title { get; internal set; }
        public string Author { get; internal set; }
        public string IconUrl { get; internal set; }
        public uint Color { get; internal set; }
        [JsonProperty]
        internal Message[] messages;

        internal EmbedResponse() { }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal override async Task Respond(ISocketMessageChannel channel)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.Title = this.Title;
            embedBuilder.Color = new Color(this.Color);
            embedBuilder.Author = new EmbedAuthorBuilder().WithName(this.Author)
                .WithIconUrl(this.IconUrl);

            foreach (Message m in this.messages)
                embedBuilder.AddField(m.title, m.content);

            await channel.SendMessageAsync(string.Empty, embed: embedBuilder.Build());
        }

        public sealed class Builder
        {
            private EmbedResponse response = new EmbedResponse();
            private List<Message> messages;

            public Builder() : this(EmbedResponseTemplate.Empty) { }

            public Builder(EmbedResponseTemplate template)
            {
                response = EmbedResponseTemplateFactory.CreateFromTemplate(template);
                if (response.messages == null)
                    this.messages = new List<Message>();
                else
                    this.messages = new List<Message>(response.messages);
            }

            public Builder SetTitle(string title)
            {
                response.Title = title;
                return this;
            }

            public Builder SetAuthor(string author)
            {
                response.Author = author;
                return this;
            }

            public Builder SetColor(byte r, byte g, byte b)
            {
                response.Color = (uint)((r << 16) + (g << 8) + b);
                return this;
            }

            public Builder SetIconUrl(string iconUrl)
            {
                response.IconUrl = iconUrl;
                return this;
            }

            public Builder AddMessage(string title, string message)
            {
                messages.Add(new Message(title, message));
                return this;
            }

            public EmbedResponse Build()
            {
                response.messages = messages.ToArray();
                messages.Clear();
                return response;
            }
        }

        internal struct Message
        {
            public string title;
            public string content;

            public Message(string title, string content)
            {
                this.title = title;
                this.content = content;
            }
        }
    }
}
