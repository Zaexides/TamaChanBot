using System;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;
using Discord.WebSocket;

namespace TamaChanBot.Core
{
    public sealed class CommandInvoker
    {
        private ResponseHandler responseHandler = new ResponseHandler();

        public async Task InvokeCommand(string commandName, MessageContext messageContext, SocketUserMessage socketMessage)
        {
            TamaChan.Instance.Logger.LogInfo($"Received command \"{commandName}\". Details:\r\n{messageContext}");
            Command command = TamaChan.Instance.CommandRegistry[commandName];
            if (command == null)
                await responseHandler.Respond(new MessageResponse($"The command \"{commandName}\" was not found."), socketMessage.Channel);
            else
            {
                //Get parameters here
                await responseHandler.Respond(command.Invoke(), socketMessage.Channel);
            }
        }
    }
}
