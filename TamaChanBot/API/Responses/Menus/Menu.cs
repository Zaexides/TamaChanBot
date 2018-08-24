using System;
using System.Linq;

namespace TamaChanBot.API.Responses
{
    public class Menu : MenuHandlerObject<int>
    {
        public readonly string[] options;

        public bool CancelOnNonOption { get; set; } = false;
        
        //TODO: add "enter any response" thingy (Not as part of the menu, probably.)
        
        public Menu(string title, ResponseHandler<int> responseHandler, params string[] options) : this(title, "", responseHandler, options)
        {
        }

        public Menu(string title, string description, ResponseHandler<int> responseHandler, params string[] options) : base(title, responseHandler, description)
        {
            if (options == null || options.Length == 0)
                throw new ArgumentException("Parameter array needs to have 1 or more items.", "options");

            this.options = options.Select(s => s.ToLowerInvariant()).Distinct().ToArray();

            this.responseHandler = responseHandler;
        }

        public override bool HandleMenuOperation(out object response, MessageContext messageContext)
        {
            if (options.Contains(messageContext.message.ToLower()))
            {
                int optionId = options.ToList().FindIndex(o => o.Equals(messageContext.message.ToLower()));
                
                response = this.responseHandler(optionId, messageContext, data);
                return true;
            }
            else if (CancelOnNonOption)
            {
                response = this.responseHandler(-1, messageContext, data);
                return true;
            }
            else
            {
                response = null;
                return false;
            }
        }
    }
}
