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
        [JsonIgnore]
        protected abstract TamaChanModule ParentModule { get; }

        protected ModuleSettings(string defaultFilePath, Logger logger) : this(defaultFilePath)
        {
            this.logger = logger;
        }

        protected ModuleSettings(string defaultFilePath) : base()
        {
            SetDefaultPath(defaultFilePath);
        }

        private void SetDefaultPath(string defaultFilePath)
        {
            this.DefaultPath = $@"Settings\Modules\{ParentModule.RegistryName}\{defaultFilePath}.json";
        }

        public static T LoadOrCreate<T>(string fileName, Logger logger = null) where T:ModuleSettings, new()
        {
            T emptySettings = new T();
            emptySettings.SetDefaultPath(fileName);

            Settings loadedSettings = emptySettings.LoadFromFile();
            if (loadedSettings != null)
                emptySettings = loadedSettings as T;
            emptySettings.SetDefaultPath(fileName);
            emptySettings.logger = logger;
            return emptySettings;
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
