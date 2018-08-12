﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Discord.WebSocket;
using TamaChanBot.API.Responses;

namespace TamaChanBot.Core
{
    public sealed class ResponseHandler
    {
        private static readonly CultureInfo EN_US = new CultureInfo("en-US");

        public async Task Respond(object response, ISocketMessageChannel channel)
        {
            if (response is Response)
                await Respond(response as Response, channel);
            else
            {
                TypeCode typeCode = Type.GetTypeCode(response.GetType());
                if(typeCode == TypeCode.Object)
                {
                    throw new NotImplementedException();
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

        public async Task Respond(Response response, ISocketMessageChannel channel)
        {
            TamaChan.Instance.Logger.LogInfo($"Responding with \"{response}\".");
            if(response is MessageResponse)
                await channel.SendMessageAsync((response as MessageResponse).content);
        }
    }
}
