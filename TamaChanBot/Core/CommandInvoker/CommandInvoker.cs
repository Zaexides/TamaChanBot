using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;
using TamaChanBot.Utility;
using Discord.WebSocket;
using Discord;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private ResponseHandler responseHandler = new ResponseHandler();
        private readonly string[] booleanTrueTerms = new string[]
        {
            "true", "yes", "1", "yeah", "yup", "positive", "alright", "ok", "okay", "yay", "enabled", "enable"
        };
        private readonly string[] booleanFalseTerms = new string[]
        {
            "false", "no", "0", "nope", "nah", "negative", "never", "nay", "disabled", "disable"
        };

        public async Task InvokeCommand(string commandName, MessageContext messageContext, SocketUserMessage socketMessage)
        {
            IDisposable typingDisposable = null;
            if (TamaChan.Instance.botSettings.sendTyping)
                typingDisposable = socketMessage.Channel.EnterTypingState();

            TamaChan.Instance.Logger.LogInfo($"Received command \"{commandName}\". Details:\r\n{messageContext}");
            Command command = TamaChan.Instance.CommandRegistry[commandName.ToLower()];
            IUser self = await socketMessage.Channel.GetUserAsync(TamaChan.Instance.GetSelf().Id);

            if (command == null)
                await SendErrorResponse("Command not found.", $"The command \"{commandName}\" was not found.", socketMessage.Channel);
            else if(socketMessage.Author is SocketGuildUser && !UserCanUseCommand(command, socketMessage.Author as SocketGuildUser, socketMessage.Channel as SocketGuildChannel))
            {
                await SendErrorResponse("Not enough permissions.", "You don't have the permission to use this command.", socketMessage.Channel);
            }
            else if(command.isNsfw && !(socketMessage.Channel is IDMChannel) && !((socketMessage.Channel is ITextChannel) && (socketMessage.Channel as ITextChannel).IsNsfw))
            {
                await SendErrorResponse("Not allowed here.", "This is a NSFW command and is only allowed in NSFW channels or DMs.", socketMessage.Channel);
            }
            else if(self is SocketGuildUser && !UserCanUseCommand(command, self as SocketGuildUser, socketMessage.Channel as SocketGuildChannel))
            {
                await SendErrorResponse("I can't do that.", "I don't have enough permissions to do that.", socketMessage.Channel);
            }
            else
            {
                try
                {
                    object[] parameters = GetParameters(messageContext.parameterString, command.method.GetParameters(), messageContext);
                    await responseHandler.Respond(command.Invoke(parameters), socketMessage.Channel);
                }
                catch (ArgumentNullException argNullEx)
                {
                    await SendErrorResponse("Invalid parameter.", argNullEx.ParamName, socketMessage.Channel);
                }
                catch (TargetParameterCountException tarParCountEx)
                {
                    await SendMissingParametersErrorResponse(command, commandName, socketMessage.Channel);
                    TamaChan.Instance.Logger.LogWarning($"Error details: {tarParCountEx.ToString()}");
                }
                catch(Exception ex)
                {
                    await SendErrorResponse("Something went wrong...", $"Please inform the bot owner of this error with as many details as possible.\nError: {ex.GetType().FullName}", socketMessage.Channel);
                    TamaChan.Instance.Logger.LogError(ex.ToString());
                }
            }

            if(typingDisposable != null)
                typingDisposable.Dispose();
        }

        private bool UserCanUseCommand(Command command, SocketGuildUser user, SocketGuildChannel channel)
        {
            if (user.GuildPermissions.Administrator)
                return true;
            if ((user.GetPermissions(channel).RawValue & (ulong)command.permissionFlag) == (ulong)command.permissionFlag)
            {
                if (!command.botOwnerOnly || TamaChan.Instance.botSettings.ownerUUIDs.Contains(user.Id))
                    return true;
            }
            return false;
        }

        private async Task SendErrorResponse(string errorTitle, string errorMessage, ISocketMessageChannel channel)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Error);
            builder.AddMessage(errorTitle, errorMessage);
            await responseHandler.Respond(builder.Build(), channel);
        }

        private async Task SendMissingParametersErrorResponse(Command command, string commandName, ISocketMessageChannel channel)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Error);
            builder.AddMessage("Not enough parameters.", "You are missing some parameters.");
            ParameterInfo[] parameters = command.method.GetParameters();
            string usage = $"{TamaChan.Instance.botSettings.commandPrefix}{commandName}";
            for(int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo pi = parameters[i];

                if (!pi.ParameterType.IsAssignableFrom(typeof(MessageContext)))
                {
                    ParameterInfo nextParameter = (i < parameters.Length - 1) ? parameters[i + 1] : null;
                    if (pi.IsOptionalParameter(nextParameter))
                        usage += $" ({pi.Name})";
                    else
                        usage += $" {pi.Name}";
                }
            }
            builder.AddMessage("Usage:", usage);
            builder.SetFooter($"Example: {command.Example}");

            await responseHandler.Respond(builder.Build(), channel);
        }

        private object[] GetParameters(string unparsedParameters, ParameterInfo[] parameters, MessageContext messageContext)
        {
            if (unparsedParameters == null)
                unparsedParameters = string.Empty;

            object[] parsedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo nextParameter = null;
                if (i < parameters.Length - 1)
                    nextParameter = parameters[i + 1];

                parsedParameters[i] = GetParameter(ref unparsedParameters, parameters[i], nextParameter, messageContext);
            }
            return parsedParameters;
        }

        private object GetParameter(ref string unparsedParameters, ParameterInfo parameterInfo, ParameterInfo nextParameter, MessageContext messageContext)
        {
            return GetParameter(ref unparsedParameters, parameterInfo.ParameterType, parameterInfo.IsOptionalParameter(nextParameter), parameterInfo.DefaultValue, nextParameter, messageContext);
        }

        private object GetParameter(ref string unparsedParameters, Type parameterType, bool isOptional, object defaultValue, ParameterInfo nextParameter, MessageContext messageContext)
        {
            TypeCode typeCode = Type.GetTypeCode(parameterType);
            if(typeCode == TypeCode.Object)
            {
                if (parameterType.IsArray)
                {
                    bool nextParameterIsNumeric = false;
                    if (nextParameter != null)
                        nextParameterIsNumeric = Type.GetTypeCode(nextParameter.ParameterType).IsNumeric();
                    return ParseArrayParameter(ref unparsedParameters, parameterType.GetElementType(), isOptional, nextParameterIsNumeric, messageContext);
                }
                else
                    return ParseObject(ref unparsedParameters, parameterType, isOptional, defaultValue, nextParameter, messageContext);
            }
            else if(typeCode == TypeCode.String)
            {
                return ParseString(ref unparsedParameters, isOptional, defaultValue, nextParameter);
            }
            else if((int)typeCode >= (int)TypeCode.Boolean && (int)typeCode <= (int)TypeCode.Decimal)
            {
                string unparsedSoleParameter = unparsedParameters;
                int soleParameterEndPosition = unparsedSoleParameter.IndexOf(' ');
                if (soleParameterEndPosition > 0)
                    unparsedSoleParameter = unparsedSoleParameter.Remove(soleParameterEndPosition, unparsedSoleParameter.Length - soleParameterEndPosition);

                if ((unparsedSoleParameter.Length + 1) >= unparsedParameters.Length)
                    unparsedParameters = string.Empty;
                else
                    unparsedParameters = unparsedParameters.Remove(0, unparsedSoleParameter.Length + 1);

                unparsedSoleParameter = new string(unparsedSoleParameter.Where(c => !char.IsWhiteSpace(c)).ToArray());
                if (typeCode == TypeCode.Boolean)
                    return ParseBooleanParameter(unparsedSoleParameter, isOptional, defaultValue);
                else
                    return GetNumericParameter(unparsedSoleParameter, isOptional, defaultValue, typeCode);
            }

            return null;
        }
    }
}
