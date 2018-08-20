using System;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;

namespace AdminModule
{
    [Module("TamaChanBot.Admin", Version = "1.0")]
    public class AdminModule : TamaChanModule, IMessageReceiver
    {
        public WordFilter wordFilter;

        public static AdminModule Instance { get; private set; }

        public override void Initialize()
        {
            Instance = this;
            wordFilter = new WordFilter();
        }

        public Task OnMessageReceived(MessageReceivedArgs messageReceivedArgs) => wordFilter.OnMessageReceived(messageReceivedArgs);
        
        [Command("WordFilter", PermissionFlag = Permission.ManageMessages, Description = "Enabled or disables the word filter on this server.")]
        [AltCommand("WF")]
        public EmbedResponse WordFilterEnableCommand(MessageContext context, bool enableOrDisable) => wordFilter.EnableCommand(context, enableOrDisable);

        [Command("WordFilterAdd", PermissionFlag = Permission.ManageMessages, Description = "Adds a word to the word filter.")]
        [AltCommand("WFAdd")]
        public EmbedResponse WordFilterAddCommand(MessageContext context, string word) => wordFilter.AddWordCommand(context, word);

        [Command("WordFilterRemove", PermissionFlag = Permission.ManageMessages, Description = "Removes a word from the world filter.")]
        [AltCommand("WFRemove")]
        public EmbedResponse WordFilterRemoveCommand(MessageContext context, string word) => wordFilter.RemoveWordCommand(context, word);
    }
}
