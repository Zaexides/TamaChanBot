using System;

namespace TamaChanBot.API
{
    public interface IMessageContextParser
    {
        void ParseMessageContext(MessageContext messageContext, bool isOptional, object defaultValue);
    }
}
