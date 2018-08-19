using System;
using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot.API
{
    public abstract class TamaChanModule
    {
        public ulong BotID
        {
            get => TamaChan.Instance.GetSelf().Id;
        }
        public string RegistryName { get; internal set; }

        public TamaChanModule()
        {
        }

        public async void SendResponse(object response, MessageContext context) => await ResponseHandler.Instance.Respond(response, context);
    }
}
