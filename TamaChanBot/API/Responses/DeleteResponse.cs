using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace TamaChanBot.API.Responses
{
    public class DeleteResponse : Response
    {
        private readonly MessageContext context;

        public DeleteResponse(MessageContext context)
        {
            this.context = context;
        }

        internal override async Task Respond(ISocketMessageChannel channel)
        {
#pragma warning disable CS0618 //Disable Obsolete warning.
            await context.wrappedMessage.DeleteAsync();
#pragma warning restore CS0618 //Disable Obsolete warning.
        }

        public override string ToString()
        {
            return "Deleting.";
        }
    }
}
