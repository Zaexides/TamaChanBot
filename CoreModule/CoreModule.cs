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
        public static TamaChanModule instance;

        public CoreModule()
        {
            instance = this;
        }

        [Command("Increment")]
        public string IncrementCommand(MessageContext context)
        {
            MyUserData myUserData = MyUserData.LoadOrCreate<MyUserData>(context.authorId, logger);
            myUserData.myValue++;
            myUserData.MarkDirty();

            return $"It's now {myUserData.myValue}.";
        }

        public class MyUserData : ModuleUserSettings
        {
            public int myValue = 0;

            protected override TamaChanModule ParentModule => CoreModule.instance;
            protected override bool IsGuildSettings => false;
            protected override byte[] EncryptionEntropy => new byte[] { 53, 69, 20, 143 };

            public MyUserData() : base() { }

            public MyUserData(ulong id, Logger logger) : base(id, logger) { }
        }
    }
}
