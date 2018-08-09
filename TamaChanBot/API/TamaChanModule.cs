using System;
using TamaChanBot.Core;

namespace TamaChanBot.API
{
    public abstract class TamaChanModule
    {
        public TamaChanModule()
        {

        }

        protected void RegisterCommand(string commandName, Command command) => TamaChan.Instance.CommandRegistry.RegisterCommand(this, commandName, command);
    }
}
