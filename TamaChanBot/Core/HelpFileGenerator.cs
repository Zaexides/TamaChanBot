using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using TamaChanBot.API;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public class HelpFileGenerator
    {
        public const string HELP_FILE_PATH = @"Data\Help.txt";
        public const string NSFW_HELP_FILE_PATH = @"data\HelpNSFW.txt";

        public List<Command> commands = new List<Command>();

        public void Generate()
        {
            TamaChan.Instance.Logger.LogInfo("Generating help files...");
            string contents = "Command List:\r\n";
            string nsfwContents = string.Empty;
            foreach (Command c in commands)
            {
                if (!c.botOwnerOnly)
                {
                    if (c.isNsfw)
                        nsfwContents += WriteCommandInfo(c) + "\r\n\r\n";
                    else
                        contents += WriteCommandInfo(c) + "\r\n\r\n";
                }
                c.Description = null;
            }

            FileInfo fileInfo = new FileInfo(HELP_FILE_PATH);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
            File.WriteAllText(HELP_FILE_PATH, contents, System.Text.Encoding.Unicode);
            contents += nsfwContents;
            File.WriteAllText(NSFW_HELP_FILE_PATH, contents, System.Text.Encoding.Unicode);
            commands.Clear();
            TamaChan.Instance.Logger.LogInfo("Help files generated.");
        }

        private string WriteCommandInfo(Command command)
        {
            string content = $"{TamaChan.Instance.botSettings.commandPrefix}{command.name}";
            ParameterInfo[] parameters = command.method.GetParameters();
            for(int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];

                if (!parameterInfo.ParameterType.IsAssignableFrom(typeof(MessageContext)))
                {
                    ParameterInfo nextParameter = (i < parameters.Length - 1) ? parameters[i + 1] : null;
                    if (parameterInfo.IsOptionalParameter(nextParameter))
                        content += $" ({parameterInfo.Name})";
                    else
                        content += $" [{parameterInfo.Name}]";
                }
            }
            if (command.isNsfw)
                content += "\r\n(NSFW)";
            content += $"\r\n{command.Description}";
            return content;
        }
    }
}
