using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TamaChanBot.API
{
    public abstract class GuildData : UserData
    {
        [JsonProperty]
        protected Dictionary<ulong, UserData> channelData = new Dictionary<ulong, UserData>();

        public void SaveChannelData(ulong channelId, UserData userData)
        {
            if (channelData.ContainsKey(channelId))
                channelData[channelId] = userData;
            else
                channelData.Add(channelId, userData);
        }

        public T GetChannelData<T>(ulong channelId) where T:UserData
        {
            if (channelData.ContainsKey(channelId))
                return (T)channelData[channelId];
            else
                return null;
        }
    }
}
