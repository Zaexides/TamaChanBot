using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class EventSystem
    {
        internal EventSystem(DiscordSocketClient client)
        {
            client.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage))
                return;

            if(arg.Content.StartsWith(TamaChan.Instance.botSettings.commandPrefix))
            {
                MessageContext context = new MessageContext(arg as SocketUserMessage);
            }
        }
    }
}
