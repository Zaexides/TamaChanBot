using System;

namespace TamaChanBot.API.Responses
{
    public class MessageResponse : Response
    {
        public readonly string content;

        public MessageResponse(string content)
        {
            this.content = content;
        }

        public override string ToString() => content;
    }
}
