using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using TamaChanBot.Core.Settings;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed class TamaChan
    {
        private const int RECONNECT_DELAY = 10000;

        private DiscordSocketClient client;

        private bool reconnect = true;

        public BotSettings botSettings;

        public static TamaChan Instance { get; private set; }
        public EventSystem EventSystem { get; private set; }
        public ModuleRegistry ModuleRegistry { get; private set; }
        public CommandRegistry CommandRegistry { get; private set; }

        public AutoSaver AutoSaver { get; private set; }
        private CancellationTokenSource autoSaverCancelTokenSource;

        internal Logger Logger { get; private set; }

        public TamaChan()
        {
            Instance = this;
            Logger = new Logger("Main");
            AutoSaver = new AutoSaver();
            botSettings = new BotSettings().LoadFromFile() as BotSettings;
            Logger.allowSystemBeeps = botSettings.allowErrorBeeps;
            client = new DiscordSocketClient();
            EventSystem = new EventSystem(client);
            ModuleRegistry = new ModuleRegistry();
            CommandRegistry = new CommandRegistry();
            ModuleRegistry.RegisterModules();

            autoSaverCancelTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = autoSaverCancelTokenSource.Token;
            Task.Run(() => AutoSaver.Run(cancellationToken), cancellationToken);
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
                    Logger.LogInfo($"Will try reconnecting in {RECONNECT_DELAY * 0.001} seconds...");
                    await Task.Delay(RECONNECT_DELAY);
                    if (reconnect)
                        Logger.LogInfo("Reconnecting...");
                }
            }

            await Stop();
        }

        public async Task Stop()
        {
            autoSaverCancelTokenSource.Cancel();
            Logger.LogInfo("Disconnecting...");
            if (client != null && (client.ConnectionState == Discord.ConnectionState.Connected || client.ConnectionState == Discord.ConnectionState.Connecting))
                await client.StopAsync();
            Logger.LogInfo("Saving settings...");
            AutoSaver.SaveAll();
            Logger.LogInfo("Stopped succesfully!");
        }

        public SocketSelfUser GetSelf()
        {
            if (client != null && client.ConnectionState == Discord.ConnectionState.Connected)
                return client.CurrentUser;
            else
                return null;
        }

        public async Task SetPlayingStatus(string status, string streamUrl = null)
        {
            Discord.ActivityType activityType = (streamUrl == null) ? Discord.ActivityType.Playing : Discord.ActivityType.Streaming;
            await client.SetGameAsync(status, streamUrl, activityType);
        }
    }
}
