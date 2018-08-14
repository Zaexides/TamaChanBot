using System;
using System.Reflection;
using System.Linq;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private bool ParseBooleanParameter(string unparsedParameter, bool isOptional, object defaultValue)
        {
            bool isEmpty = unparsedParameter == null || unparsedParameter.Equals(string.Empty);

            if (!isEmpty)
            {
                string lcPar = unparsedParameter.ToLower();
                if (booleanTrueTerms.Contains(lcPar))
                    return true;
                else if (booleanFalseTerms.Contains(lcPar))
                    return false;
            }

            if (isOptional)
                return (bool)defaultValue;
            else if (isEmpty)
                throw new TargetParameterCountException();
            else
                throw new ArgumentNullException($"\"{unparsedParameter}\" is not a valid answer.");
        }
    }
}
