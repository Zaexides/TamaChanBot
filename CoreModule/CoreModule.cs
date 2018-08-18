using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TamaChanBot;
using TamaChanBot.API;
using TamaChanBot.API.Events;
using TamaChanBot.API.Responses;
using TamaChanBot.Core.Settings;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public static Logger logger = new Logger("CoreModule");

        public CoreModule()
        {
        }

        [Command("Increment")]
        public string IncrementCommand(MessageContext context)
        {
            UserDataCore userData = GetUserData<UserDataCore>(context.authorId);
            userData.myValue++;
            SaveUserData(context.authorId, userData);

            return $"It's now {userData.myValue}.";
        }

        public class UserDataCore : UserData
        {
            public int myValue = 0;
        }
    }
}
