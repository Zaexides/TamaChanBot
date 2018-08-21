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
        internal MenuHandler MenuHandler { get; private set; }

        public delegate Task MessageReceived(MessageReceivedArgs messageReceivedArgs);
        internal event MessageReceived messageReceivedEvent;

        public delegate Task OnConnect();
        internal event OnConnect onConnectedEvent;

        public delegate Task OnDisconnect(Exception exception);
        internal event OnDisconnect onDisconnectedEvent;

        internal EventSystem(DiscordSocketClient client)
        {
            client.MessageReceived += OnMessageReceived;
            client.Connected += OnConnected;
            client.Disconnected += OnDisconnected;
            commandInvoker = new CommandInvoker();
            MenuHandler = new MenuHandler();
        }

        private async Task OnDisconnected(Exception arg)
        {
            TamaChan.Instance.Logger.LogInfo("Disconnected...");
            await onDisconnectedEvent?.Invoke(arg);
        }

        private async Task OnConnected()
        {
            TamaChan.Instance.Logger.LogInfo("Connected!");
            await onConnectedEvent?.Invoke();
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage))
                return;

            MessageContext context = new MessageContext(arg as SocketUserMessage);

            string prefix = TamaChan.Instance.botSettings.commandPrefix;
            bool isCommand = !arg.Author.IsBot && arg.Content.StartsWith(prefix);

            MessageReceivedArgs messageReceivedArgs = new MessageReceivedArgs(arg as SocketUserMessage, isCommand);
            await messageReceivedEvent?.Invoke(messageReceivedArgs);
            if (messageReceivedArgs.isCanceled)
                return;

            if(MenuHandler.IsAwaitingResponseFrom(arg.Author.Id, arg.Channel.Id))
            {
                object response = MenuHandler.HandleResponse(arg.Author.Id, arg.Channel.Id, context);
                if (response != null)
                    await ResponseHandler.Instance.Respond(response, arg.Channel);
            }
            if (isCommand)
                await commandInvoker.InvokeCommand(arg.Content.Split(' ')[0].Remove(0, prefix.Length), context, arg as SocketUserMessage);
        }
    }
}
