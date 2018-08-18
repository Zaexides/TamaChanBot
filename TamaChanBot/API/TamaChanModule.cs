using System;
using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot.API
{
    public abstract class TamaChanModule
    {
        public string RegistryName { get; internal set; }

        public TamaChanModule()
        {
        }

        protected void SaveUserData(ulong userId, UserData userData) => TamaChan.Instance.userSettings.SaveUserData(this, userId, userData);
        protected T GetUserData<T>(ulong userId) where T : UserData, new() => TamaChan.Instance.userSettings.GetUserData<T>(this, userId);
        protected void SaveGuildData(ulong guildId, UserData userData) => TamaChan.Instance.userSettings.SaveGuildData(this, guildId, userData);
        protected T GetGuildData<T>(ulong guildId) where T : UserData, new() => TamaChan.Instance.userSettings.GetGuildData<T>(this, guildId);
    }
}
