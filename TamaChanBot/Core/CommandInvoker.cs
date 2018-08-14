using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;
using Discord.WebSocket;

namespace TamaChanBot.Core
{
    public sealed class CommandInvoker
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
                catch (TargetParameterCountException)
                {
                    await SendErrorResponse("Not enough parameters.", "You're missing a couple of parameters for this command.", socketMessage.Channel);
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
                isOptional = parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
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
                    if(nextParameter != null)
                    {
                        int typeCodeIndex = (int)Type.GetTypeCode(nextParameter.ParameterType);
                        nextParameterIsNumeric = typeCodeIndex >= 5 && typeCodeIndex <= 15;
                    }
                    return ParseArrayParameter(ref unparsedParameters, parameterType.GetElementType(), isOptional, nextParameterIsNumeric);
                }
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

                if (typeCode == TypeCode.Boolean)
                    return ParseBooleanParameter(unparsedSoleParameter, isOptional, defaultValue);
                else
                    return GetNumericParameter(unparsedSoleParameter, isOptional, defaultValue, typeCode);
            }

            return null;
        }

        private Array ParseArrayParameter(ref string unparsedParameters, Type arrayType, bool isOptional, bool nextParameterIsNumeric)
        {
            TamaChan.Instance.Logger.LogInfo("Test");
            List<object> objectList = new List<object>();
            string unparsedParametersBackup = unparsedParameters;
            try
            {
                while (unparsedParameters != null && unparsedParameters.Length > 0)
                {
                    unparsedParametersBackup = unparsedParameters;
                    object member = GetParameter(ref unparsedParameters, arrayType, false, null, null);
                    if(Type.GetTypeCode(arrayType) == TypeCode.Char && nextParameterIsNumeric)
                    {
                        if(char.IsNumber((char)member))
                        {
                            unparsedParameters = unparsedParametersBackup;
                            break;
                        }
                    }
                    objectList.Add(member);
                }
            }
            catch
            {
                unparsedParameters = unparsedParametersBackup;
            }
            finally
            {

                if (objectList.Count == 0)
                {
                    if (isOptional)
                        objectList = null;
                    else
                        throw new TargetParameterCountException();
                }
            }
            if (objectList == null)
                return null;
            object[] abstractArray = objectList.ToArray();
            Array concreteArray = Array.CreateInstance(arrayType, abstractArray.Length);
            Array.Copy(abstractArray, concreteArray, concreteArray.Length);
            return concreteArray;
        }
        
        private bool ParseBooleanParameter(string unparsedParameter, bool isOptional, object defaultValue)
        {
            bool isEmpty = unparsedParameter == null || unparsedParameter.Equals(string.Empty);

            if (!isEmpty)
            {
                string lcPar = unparsedParameter.ToLower();
                if (booleanTrueTerms.Contains(lcPar))
                    return true;
                else if (booleanFalseTerms.Contains(lcPar))
                    return false;
            }

            if (isOptional)
                return (bool)defaultValue;
            else if (isEmpty)
                throw new TargetParameterCountException();
            else
                throw new ArgumentNullException($"\"{unparsedParameter}\" is not a valid answer.");
        }

        private object GetNumericParameter(string unparsedParameter, bool isOptional, object defaultValue, TypeCode typeCode)
        {
            object value;
            try
            {
                value = Convert.ChangeType(unparsedParameter, typeCode);
            }
            catch
            {
                if (isOptional)
                    value = defaultValue;
                else if (unparsedParameter == null || unparsedParameter.Equals(string.Empty))
                    throw new TargetParameterCountException();
                else
                {
                    string laymanTranslation = "number";
                    if (typeCode == TypeCode.Char)
                        laymanTranslation = "character";
                    throw new ArgumentNullException($"\"{unparsedParameter}\" is not a valid {laymanTranslation}.");
                }
            }
            return value;
        }
    }
}
