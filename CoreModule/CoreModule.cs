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
        }

        [Command("Ping")]
        public async Task PingCommand()
        {

        }
    }
}
