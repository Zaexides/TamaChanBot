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
    public class MenuResponse : BaseMenuResponse
    {
        public MenuResponse(Menu menu, MessageContext context) : base(menu, context)
        {
            this.messages = new Message[1]
            {
                new Message("Options:", string.Join("\r\n", menu.options))
            };
        }
    }
}
