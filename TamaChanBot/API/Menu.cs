using System;
using System.Linq;

namespace TamaChanBot.API
{
    public class Menu
    {
        public readonly string title;
        public readonly string[] options;
        public Response response;
        
        public Menu(string title, Response response, params string[] options)
        {
            this.title = title;
            if (options == null || options.Length == 0)
                throw new ArgumentException("Parameter array needs to have 1 or more items.", "options");
            this.options = options.Select(s => s.ToLowerInvariant()).ToArray();
            this.response = response;
        }

        public delegate object Response(int response, MessageContext context);
    }
}
