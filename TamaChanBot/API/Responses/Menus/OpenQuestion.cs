using System;

namespace TamaChanBot.API.Responses
{
    public class OpenQuestion : MenuHandlerObject<string>
    {
        public OpenQuestion(string title, ResponseHandler<string> responseHandler) : this(title, "", responseHandler)
        {
        }

        public OpenQuestion(string title, string description, ResponseHandler<string> responseHandler) : base(title, responseHandler, description)
        {
        }

        public override bool HandleMenuOperation(out object response, MessageContext messageContext)
        {
            response = this.responseHandler(messageContext.message, messageContext, data);
            return true;
        }
    }
}
