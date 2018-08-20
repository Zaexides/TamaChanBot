using System;
using TamaChanBot.API;

namespace AdminModule
{
    public class AdminModuleServerSettings : ModuleUserSettings
    {
        protected override TamaChanModule ParentModule => AdminModule.Instance;
        protected override bool IsGuildSettings => true;
        protected override byte[] EncryptionEntropy => new byte[] {23, 2, 128, 43, 68, 70};

        public bool wordFilterActive = false;
        public string[] bannedWords = new string[0];
    }
}
