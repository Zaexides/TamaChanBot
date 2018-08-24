using System;
using Discord.WebSocket;
using System.Threading.Tasks;
using TamaChanBot.API.Events;
using Newtonsoft.Json;

namespace TamaChanBot.API.Responses
{
    public abstract class Response
    {
        [JsonIgnore]
        public OnResponseSent onResponseSent;

        internal abstract Task<ResponseSentArgs> Respond(ISocketMessageChannel channel);

        public abstract override string ToString();

        [JsonIgnore]
        public object metadata = null;
        public delegate void OnResponseSent(ResponseSentArgs responseSentArgs, Response response);
    }
}
