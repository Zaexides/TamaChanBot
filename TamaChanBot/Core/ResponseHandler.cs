using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using TamaChanBot.API.Responses;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed class ResponseHandler
    {
        private static readonly CultureInfo EN_US = new CultureInfo("en-US");
        public static ResponseHandler Instance { get; private set; }

        public ResponseHandler()
        {
            Instance = this;
        }

        public async Task Respond(object response, MessageContext context)
        {
#pragma warning disable CS0618 //Disable Obsolete warning.
            await Respond(response, context.wrappedMessage.Channel);
#pragma warning restore CS0618 //Enable Obsolete warning.
        }

        internal async Task Respond(object response, ISocketMessageChannel channel)
        {
            if (response == null)
                return;

            if (response is Response)
                await Respond(response as Response, channel);
            else
            {
                TypeCode typeCode = Type.GetTypeCode(response.GetType());
                if(typeCode == TypeCode.Object)
                {
                    if (response.GetType().IsArray)
                    {
                        Array array = response as Array;
                        if (array.Length == 0)
                            await Respond(new MessageResponse("No result."), channel);
                        else if (response.GetType().GetElementType().IsAssignableFrom(typeof(Response)))
                        {
                            foreach (object r in array)
                                await Respond(r as Response, channel);
                        }
                        else
                        {
                            string responseString = string.Empty;
                            for (int i = 0; i < array.Length; i++)
                            {
                                object element = array.GetValue(i);
                                if (element != null)
                                    responseString += $"[{element.ToString()}]";
                                else
                                    responseString += "null";

                                if (i < array.Length - 1)
                                    responseString += ", ";
                            }

                            await Respond(new MessageResponse(responseString), channel);
                        }
                    }
                    else
                        await Respond(new MessageResponse(response.ToString()), channel);
                }
                else if(typeCode == TypeCode.DateTime)
                {
                    DateTime dateTime = (DateTime)response;
                    await Respond(new MessageResponse(dateTime.ToString("MMMM dd, yyyy", EN_US) + " at " + dateTime.ToString("HH:mm:ss zzz", EN_US)), channel);
                }
                else
                    await Respond(new MessageResponse(response.ToString()), channel);
            }
        }

        internal async Task Respond(Response response, ISocketMessageChannel channel)
        {
            TamaChan.Instance.Logger.LogInfo($"Responding with \"{response}\".");
            await response.Respond(channel);
        }
    }
}
