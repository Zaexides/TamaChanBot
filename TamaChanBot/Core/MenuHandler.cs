using System;
using System.Linq;
using System.Collections.Generic;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public class MenuHandler
    {
        private Dictionary<UserChannelCombo, Menu> activeMenus = new Dictionary<UserChannelCombo, Menu>();

        public bool IsAwaitingResponseFrom(ulong userId, ulong channelId) => activeMenus.ContainsKey(new UserChannelCombo(userId, channelId));

        public object HandleResponse(ulong userId, ulong channelId, MessageContext messageContext)
        {
            UserChannelCombo userChannelCombo = new UserChannelCombo(userId, channelId);
            Menu menu = activeMenus[userChannelCombo];
            if (menu.options.Contains(messageContext.message.ToLower()))
            {
                int optionId = menu.options.ToList().FindIndex(o => o.Equals(messageContext.message.ToLower()));
                activeMenus.Remove(userChannelCombo);
                return menu.response(optionId, messageContext);
            }
            else
                return null;
        }

        internal void AddOrReplaceActiveMenu(ulong userId, ulong channelId, Menu menu)
        {
            UserChannelCombo userChannelCombo = new UserChannelCombo(userId, channelId);
            if (this.activeMenus.ContainsKey(userChannelCombo))
                this.activeMenus[userChannelCombo] = menu;
            else
                this.activeMenus.Add(new UserChannelCombo(userId, channelId), menu);
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
