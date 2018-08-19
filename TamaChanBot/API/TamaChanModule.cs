using System;
using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot.API
{
    public abstract class TamaChanModule
    {
        public string RegistryName { get; internal set; }

        public TamaChanModule()
        {
        }
    }
}
