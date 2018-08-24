using System;
using System.Linq;
using System.Collections.Generic;
using TamaChanBot.API;
using TamaChanBot.API.Responses;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public class MenuHandler
    {
        private Dictionary<UserChannelCombo, IMenuHandlerObject> activeMenus = new Dictionary<UserChannelCombo, IMenuHandlerObject>();

        public bool IsAwaitingResponseFrom(ulong userId, ulong channelId) => activeMenus.ContainsKey(new UserChannelCombo(userId, channelId));

        public object HandleResponse(ulong userId, ulong channelId, MessageContext messageContext)
        {
            UserChannelCombo userChannelCombo = new UserChannelCombo(userId, channelId);
            IMenuHandlerObject menuHandlerObject = activeMenus[userChannelCombo];

            object responseObject;
            if (menuHandlerObject.HandleMenuOperation(out responseObject, messageContext))
            {
                Console.WriteLine("Yes!");
                activeMenus.Remove(userChannelCombo);
                return responseObject;
            }
            else
                return null;
        }

        internal void AddOrReplaceActiveMenu(ulong userId, ulong channelId, IMenuHandlerObject menuHandlerObject)
        {
            UserChannelCombo userChannelCombo = new UserChannelCombo(userId, channelId);
            if (this.activeMenus.ContainsKey(userChannelCombo))
                this.activeMenus[userChannelCombo] = menuHandlerObject;
            else
                this.activeMenus.Add(new UserChannelCombo(userId, channelId), menuHandlerObject);
        }

        private struct UserChannelCombo
        {
            private readonly ulong userId;
            private readonly ulong channelId;

            public UserChannelCombo(ulong userId, ulong channelId)
            {
                this.userId = userId;
                this.channelId = channelId;
            }

            public override string ToString()
            {
                return $"{userId}@{channelId}";
            }

            public override bool Equals(object obj)
            {
                if (obj is UserChannelCombo)
                {
                    UserChannelCombo other = (UserChannelCombo)obj;
                    return other.channelId.Equals(channelId) && other.userId.Equals(userId);
                }
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return userId.GetHashCode() ^ channelId.GetHashCode();
            }
        }
    }
}
