using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot
{
    class Program
    {
        private static TamaChan bot;

        public static void Main(string[] args)
        {
            bot = new TamaChan();
            System.Console.CancelKeyPress += Console_CancelKeyPress;
            Task.Run(() => bot.Start()).Wait();
        }

        private static void Console_CancelKeyPress(object sender, System.ConsoleCancelEventArgs e)
        {
            Task.Run(() => bot.Stop()).Wait();
        }
    }
}
