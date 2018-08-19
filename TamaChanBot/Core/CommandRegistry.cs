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
        private readonly Dictionary<string, string> altCommandRegistry = new Dictionary<string, string>();

        public Command this[string k]
        {
            get
            {
                if (registry.ContainsKey(k))
                    return registry[k];
                else if (altCommandRegistry.ContainsKey(k))
                    return registry[altCommandRegistry[k]];
                else
                    return null;
            }
        }

        public void RegisterModuleCommands(TamaChanModule module)
        {
            MethodInfo[] methods = module.GetType().GetMethods();

            foreach(MethodInfo m in methods)
            {
                CommandAttribute attribute = m.GetCustomAttribute<CommandAttribute>();
                if(attribute != null)
                {
                    Command command = new Command(m, attribute.PermissionFlag, attribute.BotOwnerOnly, module);
                    RegisterCommand(module, attribute.commandName, command);

                    IEnumerable<AltCommandAttribute> altCommandAttributes = m.GetCustomAttributes<AltCommandAttribute>();
                    foreach(AltCommandAttribute alt in altCommandAttributes)
                    {
                        if (!alt.commandName.IsRegistryValid())
                            TamaChan.Instance.Logger.LogWarning($"Alt command \"{alt.commandName}\" for \"{attribute.commandName}\" is not valid. Ignored.");
                        else
                            altCommandRegistry.Add(alt.commandName.ToLower(), attribute.commandName.ToLower());
                    }
                }
            }
        }

        private void RegisterCommand(TamaChanModule module, string commandName, Command command)
        {
            try
            {
                ModuleAttribute attribute = module.GetType().GetCustomAttribute<ModuleAttribute>();
                if (!commandName.IsRegistryValid())
                    throw new ArgumentException($"Command from module {attribute.moduleName} was given an invalid name: \"{commandName}\". Command names may only contain alphabetic characters and periods.");
                TamaChan.Instance.Logger.LogInfo($"Registering \"{attribute.moduleName} command \"{commandName.ToLower()}\"...");
                registry.Add(commandName.ToLower(), command);
                TamaChan.Instance.Logger.LogInfo($"Command \"{commandName.ToLower()}\" registered.");
            }
            catch (Exception ex)
            {
                TamaChan.Instance.Logger.LogError("An error occured while registering a command: " + ex.ToString());
            }
        }
    }
}
