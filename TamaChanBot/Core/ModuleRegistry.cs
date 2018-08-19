using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed class ModuleRegistry
    {
        private const string MODULE_LIBRARY_PATH = "Modules";
        private const string MODULE_LIBRARY_EXTENSION = "*.dll";
        private readonly Dictionary<string, TamaChanModule> registry = new Dictionary<string, TamaChanModule>();

        internal void RegisterModules()
        {
            if(!Directory.Exists(MODULE_LIBRARY_PATH))
            {
                Directory.CreateDirectory(MODULE_LIBRARY_PATH);
                throw new DirectoryNotFoundException($"Directory \"{MODULE_LIBRARY_PATH}\" did not exist. Created it instead. Please add in modules there.");
            }
            TamaChan.Instance.Logger.LogInfo("Registering modules...");
            HelpFileGenerator helpFileGenerator = new HelpFileGenerator();
            Assembly[] assemblies = LoadAssemblies();
            RegisterModules(assemblies, helpFileGenerator);
            helpFileGenerator.Generate();
        }

        private void RegisterModules(Assembly[] assemblies, HelpFileGenerator helpFileGenerator)
        {
            foreach(Assembly assembly in assemblies)
            {
                Type[] assemblyTypes = assembly.GetTypes();
                foreach(Type type in assemblyTypes)
                {
                    if(!type.IsAbstract && type.IsSubclassOf(typeof(TamaChanModule)))
                    {
                        ModuleAttribute moduleAttribute = type.GetCustomAttribute<ModuleAttribute>();
                        if (moduleAttribute == null)
                            TamaChan.Instance.Logger.LogWarning($"Class {type.FullName} from {assembly.FullName} inherits TamaChanModule, but does not contain the Module attribute. Module will be ignored.");
                        else if (moduleAttribute.moduleName.IsRegistryValid())
                        {
                            try
                            {
                                RegisterModule(type, moduleAttribute, helpFileGenerator);
                            }
                            catch (Exception ex)
                            {
                                TamaChan.Instance.Logger.LogError($"Could not instantiate and register module {type.FullName} from {assembly.FullName}: " + ex.ToString());
                            }
                        }
                        else
                            TamaChan.Instance.Logger.LogError($"Module class {type.FullName} from {assembly.FullName} has invalid characters in its name: \"{moduleAttribute.moduleName}\". May only contain alphabetic characters and periods.");
                    }
                }
            }
        }

        private void RegisterModule(Type moduleType, ModuleAttribute attribute, HelpFileGenerator helpFileGenerator)
        {
            if (attribute.moduleName == string.Empty || attribute.moduleName == null)
                throw new ArgumentException("Module name can not be null or an empty string.");
            if (registry.ContainsKey(attribute.moduleName))
                throw new ArgumentException($"Module registry already contains a module with the name \"{attribute.moduleName}\".");

            string logString = $"Registering Module \"{attribute.moduleName}\"";
            if (attribute.Version != null && !attribute.Version.Equals(string.Empty))
                logString += $" version \"{attribute.Version}\"";
            TamaChan.Instance.Logger.LogInfo(logString + "...");

            TamaChanModule module = Activator.CreateInstance(moduleType) as TamaChanModule;
            TamaChan.Instance.CommandRegistry.RegisterModuleCommands(module, helpFileGenerator);
            registry.Add(attribute.moduleName, module);
            RegisterModuleEventHandling(module);
            module.RegistryName = attribute.moduleName;
            TamaChan.Instance.Logger.LogInfo($"Module \"{attribute.moduleName}\" registered.");
        }

        private void RegisterModuleEventHandling(TamaChanModule module)
        {
            EventSystem eventSystem = TamaChan.Instance.EventSystem;

            if (module is IMessageReceiver)
                eventSystem.messageReceivedEvent += (module as IMessageReceiver).OnMessageReceived;
        }

        private Assembly[] LoadAssemblies()
        {
            string[] filepaths = Directory.GetFiles(MODULE_LIBRARY_PATH, MODULE_LIBRARY_EXTENSION, SearchOption.AllDirectories);
            Assembly[] assemblies = new Assembly[filepaths.Length];

            for(int i = 0; i < filepaths.Length; i++)
            {
                filepaths[i] = Environment.CurrentDirectory + @"\" + filepaths[i];
                try
                {
                    assemblies[i] = Assembly.LoadFile(filepaths[i]);
                }
                catch (FileLoadException flEx)
                {
                    TamaChan.Instance.Logger.LogError($"File at path {filepaths[i]} could not be loaded.\r\n" + flEx.ToString());
                }
                catch (BadImageFormatException bifEx)
                {
                    TamaChan.Instance.Logger.LogError($"File at path {filepaths[i]} is not a valid module file. Perhaps this was compiled using the wrong version?\r\n" + bifEx.ToString());
                }
            }

            return assemblies;
        }
    }
}
