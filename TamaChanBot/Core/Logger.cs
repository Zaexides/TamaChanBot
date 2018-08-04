using System;

namespace TamaChanBot.Core
{
    public static class Logger
    {
        private static void Log(LogType logType, object contents)
        {
            Console.ForegroundColor = GetConsoleColorFromType(logType);
            if (logType == LogType.Error)
                Console.Beep();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} [{logType.ToString()}] - {contents.ToString()}");
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

        public static void LogInfo(object content) => Log(LogType.Info, content);
        public static void LogWarning(object content) => Log(LogType.Warning, content);
        public static void LogError(object content) => Log(LogType.Error, content);

        private enum LogType
        {
            Info,
            Warning,
            Error,
        }
    }
}
