using System;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public CoreModule()
        {
        }

        [Command("Ping")]
        public MessageResponse PingCommand()
        {
            return new MessageResponse("Pong! As an object...");
        }

        [Command("now")]
        public DateTime DateTimeCommand()
        {
            return DateTime.Now;
        }
    }
}
