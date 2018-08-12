using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class EventSystem
    {
        private CommandInvoker commandInvoker;

        internal EventSystem(DiscordSocketClient client)
        {
            client.MessageReceived += OnMessageReceived;
            client.Connected += OnConnected;
            commandInvoker = new CommandInvoker();
        }

        private Task OnConnected()
        {
            TamaChan.Instance.Logger.LogInfo("Connected!");
            return Task.CompletedTask;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage))
                return;

            string prefix = TamaChan.Instance.botSettings.commandPrefix;
            if(arg.Content.StartsWith(prefix))
            {
                MessageContext context = new MessageContext(arg as SocketUserMessage);
                await commandInvoker.InvokeCommand(arg.Content.Split(' ')[0].Remove(0, prefix.Length), context, arg as SocketUserMessage);
            }
        }
    }
}
