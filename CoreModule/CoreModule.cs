using System;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;
using TamaChanBot.Core.Settings;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public static Logger logger = new Logger("CoreModule");

        public CoreModule()
        {
        }

    }
}
