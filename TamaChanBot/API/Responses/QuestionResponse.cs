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
    public class QuestionResponse : BaseMenuResponse
    {
        public QuestionResponse(OpenQuestion openQuestion, MessageContext messageContext) : base(openQuestion, messageContext)
        {
        }
    }
}
