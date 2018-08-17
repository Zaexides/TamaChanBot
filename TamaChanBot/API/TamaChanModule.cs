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
        protected T GetUserData<T>(ulong userId) where T : UserData => TamaChan.Instance.userSettings.GetUserData<T>(this, userId);
    }
}
