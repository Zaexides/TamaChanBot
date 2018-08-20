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

        public string[] aboutText = new string[0];
        public ActivitySettings activity = new ActivitySettings();

        protected override TamaChanModule ParentModule => CoreModule.Instance;

        public CoreModuleSettings() : this(null) { }
        public CoreModuleSettings(Logger logger) : base(DEFAULT_PATH, logger) { }

        internal class GoogleSettings
        {
            public string apiKey = string.Empty;
            public string searchId = string.Empty;

            [JsonIgnore]
            public bool IsEmpty
            {
                get
                {
                    return apiKey == null || searchId == null || apiKey.Equals(string.Empty) || searchId.Equals(string.Empty);
                }
            }
        }

        internal class ActivitySettings
        {
            public string playingStatus = string.Empty;
            public string streamingUrl = null;
        }
    }
}
