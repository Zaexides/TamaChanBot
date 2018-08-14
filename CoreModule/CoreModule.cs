using System;
using System.Threading.Tasks;
using TamaChanBot.API;
using TamaChanBot.API.Responses;

namespace CoreModule
{
    [Module("TamaChanBot.Core")]
    public class CoreModule : TamaChanModule
    {
        public CoreModule()
        {
        }

        [Command("Ping")]
        public MessageResponse PingCommand()
        {
            return new MessageResponse("Pong! As an object...");
        }

        [Command("Add")]
        public EmbedResponse AddCommand(int a, int b, params int[] additionalAdditions)
        {
            additionalAdditions = additionalAdditions ?? new int[0];

            string title = $"{a} + {b}";
            int result = a + b;
            foreach (int aa in additionalAdditions)
            {
                title += $" + {aa}";
                result += aa;
            }

            EmbedResponse.Builder builder = new EmbedResponse.Builder();
            builder.SetTitle("Result").AddMessage(title + " =", result.ToString());
            return builder.Build();
        }
    }
}
