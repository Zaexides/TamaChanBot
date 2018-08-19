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
        public MyModuleData moduleData;

        public CoreModule()
        {
            instance = this;
            moduleData = MyModuleData.LoadOrCreate<MyModuleData>("settings");
        }

        [Command("Increment")]
        public string IncrementCommand(MessageContext context)
        {
            MyUserData myUserData = MyUserData.LoadOrCreate<MyUserData>(context.authorId, logger);
            myUserData.myValue++;
            myUserData.MarkDirty();

            moduleData.globalValue++;
            moduleData.MarkDirty();

            return $"It's now {myUserData.myValue}. Globally it's {moduleData.globalValue}.";
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

        public class MyModuleData : ModuleSettings
        {
            public int globalValue = 12;

            protected override TamaChanModule ParentModule => CoreModule.instance;

            public MyModuleData() : base(null) { }
            public MyModuleData(Logger logger) : base("settings", logger) { }
        }
    }
}
