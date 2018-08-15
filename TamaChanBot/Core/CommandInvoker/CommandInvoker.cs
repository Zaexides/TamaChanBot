using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;
using Discord.WebSocket;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private ResponseHandler responseHandler = new ResponseHandler();
        private readonly string[] booleanTrueTerms = new string[]
        {
            "true", "yes", "1", "yeah", "yup", "positive", "alright", "ok", "okay", "yay"
        };
        private readonly string[] booleanFalseTerms = new string[]
        {
            "false", "no", "0", "nope", "nah", "negative", "never", "nay"
        };

        public async Task InvokeCommand(string commandName, MessageContext messageContext, SocketUserMessage socketMessage)
        {
            TamaChan.Instance.Logger.LogInfo($"Received command \"{commandName}\". Details:\r\n{messageContext}");
            Command command = TamaChan.Instance.CommandRegistry[commandName];
            if (command == null)
                await SendErrorResponse("Command not found.", $"The command \"{commandName}\" was not found.", socketMessage.Channel);
            else
            {
                try
                {
                    object[] parameters = GetParameters(messageContext.parameterString, command.method.GetParameters());
                    await responseHandler.Respond(command.Invoke(parameters), socketMessage.Channel);
                }
                catch (ArgumentNullException argNullEx)
                {
                    await SendErrorResponse("Invalid parameter.", argNullEx.ParamName, socketMessage.Channel);
                }
                catch (TargetParameterCountException tarParCountEx)
                {
                    await SendErrorResponse("Not enough parameters.", "You're missing a couple of parameters for this command.", socketMessage.Channel);
                    TamaChan.Instance.Logger.LogWarning($"Error details: {tarParCountEx.ToString()}");
                }
                catch(Exception ex)
                {
                    await SendErrorResponse("Something went wrong...", $"Please inform the bot owner of this error with as many details as possible.\nError: {ex.GetType().FullName}", socketMessage.Channel);
                    TamaChan.Instance.Logger.LogError(ex.ToString());
                }
            }
        }

        private async Task SendErrorResponse(string errorTitle, string errorMessage, ISocketMessageChannel channel)
        {
            EmbedResponse.Builder builder = new EmbedResponse.Builder(EmbedResponseTemplate.Error);
            builder.AddMessage(errorTitle, errorMessage);
            await responseHandler.Respond(builder.Build(), channel);
        }

        private object[] GetParameters(string unparsedParameters, ParameterInfo[] parameters)
        {
            if (unparsedParameters == null)
                unparsedParameters = string.Empty;

            object[] parsedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo nextParameter = null;
                if (i < parameters.Length - 1)
                    nextParameter = parameters[i + 1];

                parsedParameters[i] = GetParameter(ref unparsedParameters, parameters[i], nextParameter);
            }
            return parsedParameters;
        }

        private object GetParameter(ref string unparsedParameters, ParameterInfo parameterInfo, ParameterInfo nextParameter)
        {
            bool isOptional = parameterInfo.HasDefaultValue || parameterInfo.IsOptional;
            if (nextParameter == null)
                isOptional = isOptional || parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
            return GetParameter(ref unparsedParameters, parameterInfo.ParameterType, isOptional, parameterInfo.DefaultValue, nextParameter);
        }

        private object GetParameter(ref string unparsedParameters, Type parameterType, bool isOptional, object defaultValue, ParameterInfo nextParameter)
        {
            TypeCode typeCode = Type.GetTypeCode(parameterType);
            if(typeCode == TypeCode.Object)
            {
                if (parameterType.IsArray)
                {
                    bool nextParameterIsNumeric = false;
                    if (nextParameter != null)
                    {
                        int typeCodeIndex = (int)Type.GetTypeCode(nextParameter.ParameterType);
                        nextParameterIsNumeric = typeCodeIndex >= 5 && typeCodeIndex <= 15;
                    }
                    return ParseArrayParameter(ref unparsedParameters, parameterType.GetElementType(), isOptional, nextParameterIsNumeric);
                }
                else
                    return ParseObject(ref unparsedParameters, parameterType, isOptional, defaultValue, nextParameter);
            }
            else if(typeCode == TypeCode.String)
            {
                throw new NotImplementedException();
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

                unparsedSoleParameter = new string(unparsedSoleParameter.Where(c => !Char.IsWhiteSpace(c)).ToArray());
                if (typeCode == TypeCode.Boolean)
                    return ParseBooleanParameter(unparsedSoleParameter, isOptional, defaultValue);
                else
                    return GetNumericParameter(unparsedSoleParameter, isOptional, defaultValue, typeCode);
            }

            return null;
        }
    }
}
