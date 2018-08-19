using System;
using System.IO;
using System.Collections.Generic;
using TamaChanBot.Core.Settings;
using Newtonsoft.Json;

namespace TamaChanBot.API
{
    public abstract class ModuleUserSettings : UserSettings
    {
        [JsonIgnore]
        protected Logger logger;

        [JsonIgnore]
        private static readonly Dictionary<string, ModuleUserSettings> unsavedDirtyUserSettings = new Dictionary<string, ModuleUserSettings>();

        [JsonIgnore]
        protected abstract TamaChanModule ParentModule { get; }
        [JsonIgnore]
        protected abstract bool IsGuildSettings { get; }

        internal override sealed string RegistrySub => $"{ParentModule.RegistryName.ToUpper()}\\{(IsGuildSettings ? "G":"U")}{id}";

        private ulong id;

        protected ModuleUserSettings(ulong id, Logger logger) : this(id)
        {
            this.logger = logger;
        }

        protected ModuleUserSettings(ulong id) : base()
        {
            SetDefaultPathAndId(id);
        }

        protected ModuleUserSettings() : base() { }

        private void SetDefaultPathAndId(ulong id)
        {
            this.DefaultPath = $@"Settings\{(IsGuildSettings ? "Guild" : "User")}\{ParentModule.RegistryName}\{id}";
            this.id = id;
        }

        public static T LoadOrCreate<T>(ulong id, Logger logger = null) where T: ModuleUserSettings, new()
        {
            T emptySettings = new T();
            emptySettings.logger = logger;
            emptySettings.SetDefaultPathAndId(id);
            if (unsavedDirtyUserSettings.ContainsKey(emptySettings.RegistrySub))
                return unsavedDirtyUserSettings[emptySettings.RegistrySub] as T;

            UserSettings userSettings = emptySettings.LoadFromFile() as UserSettings;
            if (userSettings == null)
                return emptySettings;
            else
            {
                T moduleUserSettings = userSettings as T;
                moduleUserSettings.logger = logger;
                moduleUserSettings.SetDefaultPathAndId(id);
                return userSettings as T;
            }
        }

        public override void MarkDirty()
        {
            base.MarkDirty();
            if(!unsavedDirtyUserSettings.ContainsKey(RegistrySub))
                unsavedDirtyUserSettings.Add(RegistrySub, this);
        }

        public void OnSave()
        {
            if(unsavedDirtyUserSettings.ContainsKey(RegistrySub))
                unsavedDirtyUserSettings.Remove(RegistrySub);
        }
    }
}
