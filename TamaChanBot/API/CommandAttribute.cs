using System;

namespace TamaChanBot.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public readonly string commandName;

        public string Description { get; set; }
        public Permission PermissionFlag { get; set; }
        public bool BotOwnerOnly { get; set; }
        public bool IsNSFW { get; set; }
        public string ParameterExample { get; set; }

        public CommandAttribute(string commandName)
        {
            this.commandName = commandName;
        }
    }
}
