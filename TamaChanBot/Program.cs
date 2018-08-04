using System.Threading.Tasks;
using TamaChanBot.Core;

namespace TamaChanBot
{
    class Program
    {
        public static void Main(string[] args) => Task.Run(() => new TamaChan().Start()).Wait();
    }
}
