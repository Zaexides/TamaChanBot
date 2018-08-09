using System;
using System.Threading.Tasks;
using TamaChanBot.API;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public CoreModule()
        {
            RegisterCommand("Ping", new PingCommand());
        }
    }

    public class PingCommand : Command
    {
        public override async Task Execute(MessageContext context, params object[] args)
        {
        }
    }
}
