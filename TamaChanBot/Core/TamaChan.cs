using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using TamaChanBot.Core.Settings;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed class TamaChan
    {
        private const string BOT_SETTINGS_PATH = @"Settings\bot_settings.json";

        private DiscordSocketClient client;
        private CommandService commandService;

        public BotSettings botSettings;

        public TamaChan Instance { get; private set; }

        public TamaChan()
        {
            Instance = this;
            botSettings = new BotSettings().LoadFromFile(BOT_SETTINGS_PATH);
        }

        public async Task Start()
        {
            if (client != null && (client.ConnectionState == Discord.ConnectionState.Connected || client.ConnectionState == Discord.ConnectionState.Connecting))
                return; //Cancel out when already running.

            Logger.LogInfo("Initializing new client...");
            client = new DiscordSocketClient();
            
            try
            {
                Logger.LogInfo("Connecting...");
                await client.LoginAsync(Discord.TokenType.Bot, botSettings.botToken);
                await client.StartAsync();
            }
            catch(Exception ex)
            {
                Logger.LogError("An error occured: " + ex.ToString());
            }

            await Task.Delay(-1);
        }
    }
}
