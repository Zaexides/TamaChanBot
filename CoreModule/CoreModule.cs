using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TamaChanBot;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule, IMessageReceiver
    {
        public static Logger logger = new Logger("CoreModule");
        public static TamaChanModule instance;
        internal CoreModuleSettings settings;

        private GoogleCommand googleCommand;
        private WikipediaSearchCommand wikipediaCommand;

        public CoreModule()
        {
            instance = this;
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

        [Command("Ping")]
        public string PingCommand(MessageContext context)
        {
            return $"Pong {((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds()}";
        }

        [Command("Google")]
        public EmbedResponse GoogleCommand(string query) => googleCommand.Execute(query);
        [Command("Wikipedia")]
        [AltCommand("Wiki")]
        public EmbedResponse WikiCommand(string query) => wikipediaCommand.Execute(query);
    }
}
