using System;
using TamaChanBot;
using TamaChanBot.API;
using Newtonsoft.Json;

namespace CoreModule
{
    internal class CoreModuleSettings : ModuleSettings
    {
        public const string DEFAULT_PATH = "Settings";

        public GoogleSettings google = new GoogleSettings();

        protected override TamaChanModule ParentModule => CoreModule.instance;

        public CoreModuleSettings() : this(null) { }
        public CoreModuleSettings(Logger logger) : base(DEFAULT_PATH, logger) { }

        internal class GoogleSettings
        {
            public string apiKey;
            public string searchId;

            [JsonIgnore]
            public bool IsEmpty
            {
                get
                {
                    return apiKey == null || searchId == null || apiKey.Equals(string.Empty) || searchId.Equals(string.Empty);
                }
            }
        }
    }
}
