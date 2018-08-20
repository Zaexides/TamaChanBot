using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;

namespace AdminModule
{
    public class WordFilter
    {
        private AdminModuleServerSettings GetSettings(ulong id) => AdminModuleServerSettings.LoadOrCreate<AdminModuleServerSettings>(id);

        public async Task OnMessageReceived(MessageReceivedArgs args)
        {
            if(args.serverId != null)
            {
                AdminModuleServerSettings serverSettings = GetSettings((ulong)args.serverId);
                if(serverSettings.wordFilterActive)
                {
                    if(serverSettings.bannedWords.Any(args.content.ToLower().Contains))
                    {
                        MessageContext context = args.CreateMessageContext();
                        if (!context.authorPermissions.HasFlag(Permission.Admin) && !context.authorPermissions.HasFlag(Permission.ManageMessages))
                        {
                            DeleteResponse deleteResponse = new DeleteResponse(context);
                            await AdminModule.Instance.SendResponse(deleteResponse, context);
                        }
                    }
                }
            }
        }

        public EmbedResponse EnableCommand(MessageContext context, bool enable)
        {
            if (context.serverId == null)
                return CreateNotAServerMessage();
            SetEnabled((ulong)context.serverId, enable);
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Info);
            builder.SetTitle("WordFilter");
            builder.AddMessage("WordFilter has been set to:", enable.ToString());
            return builder.Build();
        }

        public EmbedResponse AddWordCommand(MessageContext context, string word)
        {
            if (context.serverId == null)
                return CreateNotAServerMessage();

            EmbedResponse.Builder builder;
            if (AddWord(context.serverId, word))
            {
                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);
                builder.AddMessage("Added word to WordFilter:", word);
            }
            else
            {
                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Failure);
                builder.AddMessage($"Could not add \"{word}\" to WordFilter.", "The word is already being filtered.");
            }
            builder.SetTitle("WordFilter");
            return builder.Build();
        }

        public EmbedResponse RemoveWordCommand(MessageContext context, string word)
        {
            if (context.serverId == null)
                return CreateNotAServerMessage();

            EmbedResponse.Builder builder;
            if (RemoveWord(context.serverId, word))
            {
                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Success);
                builder.AddMessage("Removed word from WordFilter:", word);
            }
            else
            {
                builder = new EmbedResponse.Builder(EmbedResponseTemplate.Failure);
                builder.AddMessage($"Could not remove \"{word}\" from WordFilter.", "The word is not on the list to begin with.");
            }
            builder.SetTitle("WordFilter");
            return builder.Build();
        }

        private EmbedResponse CreateNotAServerMessage()
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Error);
            builder.AddMessage("Not a server!", "WordFilter commands can only be used on a server!");
            return builder.Build();
        }

        private void SetEnabled(ulong? serverId, bool enabled)
        {
            AdminModuleServerSettings serverSettings = GetSettings((ulong)serverId);
            serverSettings.wordFilterActive = enabled;
            serverSettings.MarkDirty();
        }

        private bool AddWord(ulong? serverId, string word)
        {
            AdminModuleServerSettings serverSettings = GetSettings((ulong)serverId);
            List<string> bannedWordsList = new List<string>(serverSettings.bannedWords);
            if(bannedWordsList.Contains(word.ToLower()))
                return false;
            bannedWordsList.Add(word.ToLower());
            serverSettings.bannedWords = bannedWordsList.ToArray();
            serverSettings.MarkDirty();
            return true;
        }

        private bool RemoveWord(ulong? serverId, string word)
        {
            AdminModuleServerSettings serverSettings = GetSettings((ulong)serverId);
            List<string> bannedWordsList = new List<string>(serverSettings.bannedWords);
            if (!bannedWordsList.Remove(word.ToLower()))
                return false;
            serverSettings.bannedWords = bannedWordsList.ToArray();
            serverSettings.MarkDirty();
            return true;
        }
    }
}
