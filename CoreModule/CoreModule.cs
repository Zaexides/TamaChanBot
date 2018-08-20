﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TamaChanBot;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule, IMessageReceiver, IConnectionStatusReceiver
    {
        private const string INVITE_LINK_FORMAT = "https://discordapp.com/oauth2/authorize?client_id={0}&scope=bot&permissions=-1";
        internal CoreModuleSettings settings;

        private GoogleCommand googleCommand;
        private WikipediaSearchCommand wikipediaCommand;

        public static CoreModule Instance { get; private set; }
        public static Logger Logger { get; } = new Logger("CoreModule");

        public override void Initialize()
        {
            Instance = this;
            settings = CoreModuleSettings.LoadOrCreate<CoreModuleSettings>(CoreModuleSettings.DEFAULT_PATH);
            googleCommand = new GoogleCommand(settings.google);
            wikipediaCommand = new WikipediaSearchCommand();
        }

        public async Task OnMessageReceived(MessageReceivedArgs messageReceivedArgs)
        {
            if(messageReceivedArgs.userId.Equals(BotID))
            {
                string[] splitContent = messageReceivedArgs.content.Split(' ');
                if(splitContent.Length == 2 && splitContent[0] == "Pong")
                {
                    long oldTimestamp = long.Parse(splitContent[1]);
                    long newTimestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                    long difference = newTimestamp - oldTimestamp;

                    MessageContext context = messageReceivedArgs.CreateMessageContext();
                    DeleteResponse deleteResponse = new DeleteResponse(context);
                    await SendResponse(deleteResponse, context);
                    EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Info);
                    builder.AddMessage("Pong!", $"My latency is {difference}ms.");
                    await SendResponse(builder.Build(), context);
                }
            }
        }

        [Command("Ping", Description = "Shows the bot's latency.")]
        public string PingCommand()
        {
            return $"Pong {((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds()}";
        }

        [Command("Help", Description = "Sends the help file. In NSFW channels or DMs, it'll also show NSFW commands.")]
        public MessageAttachementResponse HelpCommand(MessageContext messageContext)
        {
            if (messageContext.isNSFWChannel)
                return new MessageAttachementResponse(string.Empty, TamaChanBot.Core.HelpFileGenerator.NSFW_HELP_FILE_PATH);
            else
                return new MessageAttachementResponse(string.Empty, TamaChanBot.Core.HelpFileGenerator.HELP_FILE_PATH);
        }

        [Command("Invite", Description = "Sends an invite link.")]
        public string InviteCommand()
        {
            return string.Format(INVITE_LINK_FORMAT, BotID);
        }

        [Command("About", Description = "Shows information about the bot.")]
        public EmbedResponse AboutCommand()
        {
            string[] aboutText = settings.aboutText;

            EmbedResponse.Builder builder;
            if(aboutText == null || aboutText.Length == 0)
            {
                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Warning);
                builder.AddMessage("About TamaChanBot", "No about text was set.");
                return builder.Build();
            }
            else
            {
                string joinedText = string.Join("\r\n", aboutText);

                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Info);
                builder.AddMessage($"About TamaChanBot", joinedText);
                return builder.Build();
            }
        }

        [Command("Google", Description = "Searches the specified query on Google.")]
        public EmbedResponse GoogleCommand(string query) => googleCommand.Execute(query);
        [Command("Wikipedia", Description = "Looks up the specified article query on Wikipedia.")]
        [AltCommand("Wiki")]
        public EmbedResponse WikiCommand(string query) => wikipediaCommand.Execute(query);

        [Command("SetPlaying", BotOwnerOnly = true)]
        public EmbedResponse SetPlayingCommand(string playingStatus)
        {
            return SetPlayingStatus(null, playingStatus);
        }

        [Command("SetStreaming", BotOwnerOnly = true)]
        public EmbedResponse SetStreamingCommand(URL streamingUrl, string playingStatus)
        {
            return SetPlayingStatus(streamingUrl, playingStatus);
        }

        private EmbedResponse SetPlayingStatus(URL? streamingUrl, string playingStatus)
        {
            settings.activity.playingStatus = playingStatus;
            settings.activity.streamingUrl = streamingUrl.HasValue ? streamingUrl.Value.uri.ToString() : null;
            Console.WriteLine(settings.activity.streamingUrl);
            settings.MarkDirty();
            Task.Run(() => UpdatePlayingStatus());

            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);
            builder.AddMessage("Playing status updated succesfully!", playingStatus);
            return builder.Build();
        }
        
        private async Task UpdatePlayingStatus()
        {
            await TamaChanBot.Core.TamaChan.Instance.SetPlayingStatus(settings.activity.playingStatus, settings.activity.streamingUrl);
        }

        public async Task OnConnected()
        {
            await UpdatePlayingStatus();
        }

        public async Task OnDisconnected(Exception exception)
        {
            await Task.CompletedTask;
        }
    }
}
