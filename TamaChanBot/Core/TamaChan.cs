using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TamaChanBot.Core.Settings;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed class TamaChan
    {
        private const string BOT_SETTINGS_PATH = @"Settings\bot_settings.json";
        private const int RECONNECT_DELAY = 1000;

        private DiscordSocketClient client;

        private bool reconnect = true;

        public BotSettings botSettings;

        public static TamaChan Instance { get; private set; }
        public EventSystem EventSystem { get; private set; }
        public ModuleRegistry ModuleRegistry { get; private set; }
        public CommandRegistry CommandRegistry { get; private set; }
        
        internal Logger Logger { get; private set; }

        public TamaChan()
        {
            Instance = this;
            Logger = new Logger("Main");
            botSettings = new BotSettings().LoadFromFile(BOT_SETTINGS_PATH);
            client = new DiscordSocketClient();
            EventSystem = new EventSystem(client);
            ModuleRegistry = new ModuleRegistry();
            CommandRegistry = new CommandRegistry();
            ModuleRegistry.RegisterModules();
        }

        public async Task Start()
        {
            if (client.ConnectionState == Discord.ConnectionState.Connected || client.ConnectionState == Discord.ConnectionState.Connecting)
                return; //Cancel out when already running.

            Logger.LogInfo("Initializing new client...");

            while (reconnect)
            {
                try
                {
                    Logger.LogInfo("Connecting...");
                    await client.LoginAsync(Discord.TokenType.Bot, botSettings.botToken);
                    await client.StartAsync();
                    await Task.Delay(-1);
                }
                catch (Exception ex)
                {
                    Logger.LogError("An error occured: " + ex.ToString());
                    await Task.Delay(RECONNECT_DELAY);
                    if (reconnect)
                        Logger.LogInfo("Reconnecting...");
                }
            }

            await Stop();
        }

        public async Task Stop()
        {
            Logger.LogInfo("Disconnecting...");
            if (client != null && (client.ConnectionState == Discord.ConnectionState.Connected || client.ConnectionState == Discord.ConnectionState.Connecting))
                await client.StopAsync();
            Logger.LogInfo("Saving settings...");
            if (botSettings != null)
            {
                Logger.LogInfo("Saving Bot Settings...");
                botSettings.SaveToFile(BOT_SETTINGS_PATH);
            }
            Logger.LogInfo("Stopped succesfully!");
        }
    }
}
