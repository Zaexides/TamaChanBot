using System;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class Command
    {
        public readonly MethodInfo method;
        public readonly Permission permissionFlag;
        public readonly TamaChanModule module;

        internal Command(MethodInfo method, Permission permissionFlag, TamaChanModule module)
        {
            this.method = method;
            this.permissionFlag = permissionFlag;
            this.module = module;
        }

        public object Invoke(params object[] pars) => method.Invoke(module, pars);
    }
}
