using System;
using System.Linq;
using System.Reflection;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public partial class CommandInvoker
    {
        public object ParseObject(ref string unparsedParameters, Type parameterType, bool isOptional, object defaultValue, ParameterInfo nextParameter, MessageContext messageContext)
        {
            if (parameterType.Equals(typeof(MessageContext)))
                return messageContext;
            else if (parameterType.GetInterfaces().Contains(typeof(IParameterParser)))
            {
                object instance = Activator.CreateInstance(parameterType);
                (instance as IParameterParser).ParseParameter(ref unparsedParameters, isOptional, defaultValue, nextParameter);
                return instance;
            }
            else if(parameterType.GetInterfaces().Contains(typeof(IMessageContextParser)))
            {
                object instance = Activator.CreateInstance(parameterType);
                (instance as IMessageContextParser).ParseMessageContext(messageContext, isOptional, defaultValue);
                return instance;
            }
            else
                throw new InvalidCastException($"Can't cast \"{parameterType.FullName}\" to \"{typeof(IParameterParser).FullName}\".");
        }
    }
}
