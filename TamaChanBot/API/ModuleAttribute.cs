using System;

namespace TamaChanBot.API
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ModuleAttribute : Attribute
    {
        public readonly string moduleName;

        public string Version { get; set; }

        public ModuleAttribute(string moduleName)
        {
            this.moduleName = moduleName;
        }
    }
}
