﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;
using TamaChanBot.Core.Settings;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public static Logger logger = new Logger("CoreModule");
        public static CoreModuleSettings coreModuleSettings = ModuleSettings.LoadOrCreate<CoreModuleSettings>();

        public CoreModule()
        {
        }

        [Command("Echo")]
        public MessageResponse EchoCommand(string message)
        {
            coreModuleSettings.messages++;
            coreModuleSettings.MarkDirty();
            return new MessageResponse(message);
        }

        [Command("Ping")]
        public MessageResponse PingCommand(MessageContext context)
        {
            UserDataCoreModule userDataCoreModule = GetUserData<UserDataCoreModule>(context.authorId);
            userDataCoreModule = userDataCoreModule ?? new UserDataCoreModule();

            userDataCoreModule.pinged++;

            SaveUserData(context.authorId, userDataCoreModule);

            string response = $"Pong! You've pinged {userDataCoreModule.pinged} times.";

            if(context.IsInServer)
            {
                UserDataCoreModule userDataCoreModuleGuild = GetGuildData<UserDataCoreModule>((ulong)context.serverId);
                userDataCoreModuleGuild = userDataCoreModuleGuild ?? new UserDataCoreModule();
                userDataCoreModuleGuild.pinged++;
                SaveGuildData((ulong)context.serverId, userDataCoreModuleGuild);
                response += $" This command has been used {userDataCoreModuleGuild.pinged} times in this server now.";
            }

            return new MessageResponse(response);
        }

        [Command("Add")]
        public EmbedResponse AddCommand(int a, int b, params int[] additionalAdditions)
        {
            additionalAdditions = additionalAdditions ?? new int[0];

            string title = $"{a} + {b}";
            int result = a + b;
            foreach (int aa in additionalAdditions)
            {
                title += $" + {aa}";
                result += aa;
            }

            EmbedResponse.Builder builder = new EmbedResponse.Builder();
            builder.SetTitle("Result").AddMessage(title + " =", result.ToString());
            return builder.Build();
        }

        [Command("Shift")]
        public string ShiftCommand(ByteShiftArgument argumentObject, bool overflow = false)
        {
            return $"Result of ({argumentObject.value} {(argumentObject.toLeft ? "<<" : ">>")} {argumentObject.amount}) {(overflow ? "with" : "without")} overflow: {argumentObject.Execute(overflow)}.";
        }

        public class ByteShiftArgument : IParameterParser
        {
            public byte value, amount;
            public bool toLeft;

            public byte Execute(bool overflow)
            {
                if (toLeft)
                {
                    byte result = (byte)(value << amount);
                    if (overflow)
                        result |= (byte)(value >> (8 - amount));
                    return result;
                }
                else
                {
                    byte result = (byte)(value >> amount);
                    if (overflow)
                        result |= (byte)(value << (8 - amount));
                    return result;
                }
            }

            public void ParseParameter(ref string remainingParameters, bool isOptional, object defaultValue, ParameterInfo nextParameter)
            {
                string parameter = remainingParameters.Split(' ')[0];
                remainingParameters = remainingParameters.Remove(0, parameter.Length);

                string[] split = parameter.Split(new string[] { ">>", "<<" }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                    throw new ArgumentNullException("Parameter must be formatted as \"(value)>>(amount)\" or \"(value)<<(amount)\".");

                if (byte.TryParse(split[0], out value) && byte.TryParse(split[1], out amount))
                {
                    toLeft = parameter.Contains("<<");
                }
                else
                    throw new ArgumentNullException($"\"{split[0]}\" and \"{split[1]}\" aren't both numbers ranging from 0-255.");
            }
        }

        public class UserDataCoreModule : UserData
        {
            public int pinged = 0;
        }

        public class CoreModuleSettings : ModuleSettings
        {
            public int messages = 0;

            public CoreModuleSettings() : this(null)
            {
            }

            protected CoreModuleSettings(Logger logger) : base(@"CoreSettings", logger)
            {
            }
        }
    }
}
