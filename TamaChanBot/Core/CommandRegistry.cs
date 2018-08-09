using System;
using System.Collections.Generic;
using System.Reflection;
using TamaChanBot.API;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed class CommandRegistry
    {
        private readonly Dictionary<string, Command> registry = new Dictionary<string, Command>();

        public void RegisterCommand(TamaChanModule module, string commandName, Command command)
        {
            try
            {
                ModuleAttribute attribute = module.GetType().GetCustomAttribute<ModuleAttribute>();
                if (!commandName.IsRegistryValid())
                    throw new ArgumentException($"Command from module {attribute.moduleName} was given an invalid name: \"{commandName}\". Command names may only contain alphabetic characters and periods.");

                command.RegistryName = $"{commandName.ToLower()}@{attribute.moduleName}";
                TamaChan.Instance.Logger.LogInfo($"Registering \"{attribute.moduleName} command \"{command.RegistryName}\"...");
                registry.Add(command.RegistryName, command);
                command.OnRegister();
                TamaChan.Instance.Logger.LogInfo($"Command \"{command.RegistryName}\" registered.");
            }
            catch (Exception ex)
            {
                TamaChan.Instance.Logger.LogError("An error occured while registering a command: " + ex.ToString());
            }
        }
    }
}
