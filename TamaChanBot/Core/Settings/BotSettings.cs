using System;
using System.IO;
using Newtonsoft.Json;
using TamaChanBot.Utility;

namespace TamaChanBot.Core.Settings
{
    public sealed class BotSettings : Settings
    {
        public BotToken botToken;
        public string commandPrefix = "t.";
        public bool allowErrorBeeps = true;
        public string botName = "Tama Chan";
        public ulong[] ownerUUIDs = new ulong[] { 0 };

        public bool flipMe = false;

        public BotSettings()
        {
            this.DefaultPath = @"Settings\bot_settings.json";
        }

        public override Settings LoadFromFile(string filepath)
        {
            BotSettings botSettings = null;
            try
            {
                if (File.Exists(filepath))
                {
                    botSettings = JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText(filepath));
                }
            }
            catch(Exception ex)
            {
                TamaChan.Instance.Logger.LogError("Failed to deserialize BotSettings class: " + ex.ToString());
            }
            return botSettings == null ? new BotSettings() : botSettings;
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
            catch(Exception ex)
            {
                TamaChan.Instance.Logger.LogError("Failed to serialize BotSettings class: " + ex.ToString());
            }
        }
        
        public sealed class BotToken
        {
            [JsonProperty]
            internal string token = string.Empty;

            [JsonIgnore]
            public bool IsEmtpy
            {
                get => (token == null || token.Equals(String.Empty));
            }

            public override string ToString()
            {
                return token;
            }

            public static implicit operator String(BotToken botToken)
            {
                if (botToken.IsEmtpy)
                    throw new TokenNotSetException("Bot token was not set.");
                return botToken.token;
            }
        }
    }
}
