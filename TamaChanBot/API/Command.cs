using System;
using Discord;
using System.Threading.Tasks;

namespace TamaChanBot.API
{
    public abstract class Command
    {
        public virtual bool CanExecute(GuildPermission permissions, MessageContext context)
        {
            return true;
        }

        public abstract Task Execute(MessageContext context);
    }
}
