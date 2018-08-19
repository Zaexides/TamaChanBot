using System;

namespace TamaChanBot.API
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AltCommandAttribute : Attribute
    {
        public readonly string commandName;

        public AltCommandAttribute(string commandName)
        {
            this.commandName = commandName;
        }
    }
}
