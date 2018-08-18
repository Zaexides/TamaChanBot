using System;

namespace TamaChanBot
{
    public class Logger
    {
        private readonly string loggerName;
        public bool allowSystemBeeps = true;

        public Logger(string name)
        {
            this.loggerName = name;
        }

        private void Log(LogType logType, object contents)
        {
            Console.ForegroundColor = GetConsoleColorFromType(logType);
            if (allowSystemBeeps && logType == LogType.Error)
                Console.Beep();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} [{logType.ToString()}] [{loggerName}] - {contents.ToString()}");
        }

        private static ConsoleColor GetConsoleColorFromType(LogType type)
        {
            switch(type)
            {
                case LogType.Info:
                    return ConsoleColor.White;
                case LogType.Warning:
                    return ConsoleColor.Yellow;
                case LogType.Error:
                    return ConsoleColor.Red;
                default:
                    return ConsoleColor.White;
            }
        }

        public void LogInfo(object content) => Log(LogType.Info, content);
        public void LogWarning(object content) => Log(LogType.Warning, content);
        public void LogError(object content) => Log(LogType.Error, content);

        private enum LogType
        {
            Info,
            Warning,
            Error,
        }
    }
}
