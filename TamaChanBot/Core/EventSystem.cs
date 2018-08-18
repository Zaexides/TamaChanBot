using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using TamaChanBot.API;
using TamaChanBot.API.Events;

namespace TamaChanBot.Core
{
    public sealed class EventSystem
    {
        private CommandInvoker commandInvoker;

        public delegate Task MessageReceived(MessageReceivedArgs messageReceivedArgs);
        internal event MessageReceived messageReceivedEvent;

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

            MessageContext context = new MessageContext(arg as SocketUserMessage);

            string prefix = TamaChan.Instance.botSettings.commandPrefix;
            bool isCommand = !arg.Author.IsBot && arg.Content.StartsWith(prefix);

            messageReceivedEvent?.Invoke(new MessageReceivedArgs(arg as SocketUserMessage, isCommand));

            if (isCommand)
                await commandInvoker.InvokeCommand(arg.Content.Split(' ')[0].Remove(0, prefix.Length), context, arg as SocketUserMessage);
        }
    }
}
