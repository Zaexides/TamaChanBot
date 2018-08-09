using System;
using Discord;
using System.Threading.Tasks;

namespace TamaChanBot.API
{
    public abstract class Command
    {
        public string RegistryName { get; internal set; }

        //public abstract bool CanExecute(GuildPermission permissions, MessageContext context);
        public abstract Task Execute(MessageContext context, params object[] args);

        public virtual void OnRegister() { }
    }
}
