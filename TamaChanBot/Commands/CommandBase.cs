using System;
using Discord;
using TamaChanBot.Utility;
using System.Threading.Tasks;

namespace TamaChanBot.Commands
{
    public abstract class CommandBase
    {
        public virtual bool CanExecute(GuildPermission permissions, MessageContext context)
        {
            return true;
        }

        public abstract Task Execute(MessageContext context);
    }
}
