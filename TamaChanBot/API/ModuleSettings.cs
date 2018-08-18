using System;
using System.IO;
using TamaChanBot.Core.Settings;
using Newtonsoft.Json;

namespace TamaChanBot.API
{
    public abstract class ModuleSettings : Settings
    {
        [JsonIgnore]
        protected Logger logger;

        protected ModuleSettings(string defaultFilePath, Logger logger) : this(defaultFilePath)
        {
            this.logger = logger;
        }

        protected ModuleSettings(string defaultFilePath) : base()
        {
            this.DefaultPath = $@"Settings\Modules\{defaultFilePath}.json";
        }

        public static T LoadOrCreate<T>() where T:ModuleSettings, new()
        {
            T emptySettings = new T();
            return emptySettings.LoadFromFile() as T;
        }

        public override Settings LoadFromFile(string filepath)
        {
            ModuleSettings moduleSettings = null;
            try
            {
                if (File.Exists(filepath))
                {
                    moduleSettings = JsonConvert.DeserializeObject(File.ReadAllText(filepath), GetType()) as ModuleSettings;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError($"Failed to deserialize \"{GetType().FullName}\" class: " + ex.ToString());
            }

            return moduleSettings;
        }

        public override void SaveToFile(string filepath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filepath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                File.WriteAllText(filepath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                logger?.LogError($"Failed to serialize \"{GetType().FullName}\" class: " + ex.ToString());
            }
        }
    }
}
