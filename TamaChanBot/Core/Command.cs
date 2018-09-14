using System;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class Command
    {
        public readonly string name;
        public readonly MethodInfo method;
        public readonly Permission permissionFlag;
        public readonly bool botOwnerOnly;
        public readonly bool isNsfw;
        public readonly TamaChanModule module;
        private readonly string example;
        
        public string Description { get; set; }

        public string Example
        {
            get => $"{TamaChan.Instance.botSettings.commandPrefix}{name} {example}";
        }

        internal Command(string name, MethodInfo method, Permission permissionFlag, bool botOwnerOnly, bool isNsfw, string example, TamaChanModule module)
        {
            this.name = name;
            this.method = method;
            this.permissionFlag = permissionFlag;
            this.botOwnerOnly = botOwnerOnly;
            this.isNsfw = isNsfw;
            this.example = example;
            this.module = module;
        }

        public object Invoke(params object[] pars) => method.Invoke(module, pars);
    }
}
