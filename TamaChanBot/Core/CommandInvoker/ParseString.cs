using System;
using System.Reflection;
using TamaChanBot.Utility;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private object ParseString(ref string unparsedParameters, bool isOptional, object defaultValue, ParameterInfo nextParameter)
        {
            bool locked = false;
            char lockCharacter = '\"';
            bool escape = false;
            TypeCode nextParameterTypeCode = nextParameter != null ? Type.GetTypeCode(nextParameter.ParameterType) : TypeCode.Empty;
            string result = string.Empty;
            int removeCharacters = 0;

            for(int i = 0; i < unparsedParameters.Length; i++)
            {
                bool writeCharacter = true;
                if ((i == 0 || locked) && !escape && (unparsedParameters[i].Equals('\"') || unparsedParameters[i].Equals('\'')))
                {
                    writeCharacter = false;
                    if (!locked)
                    {
                        locked = true;
                        lockCharacter = unparsedParameters[i];
                    }
                    else if (unparsedParameters[i].Equals(lockCharacter))
                    {
                        removeCharacters++;
                        break;
                    }
                    else
                        writeCharacter = true;
                }
                else if (!locked && i > 0 && char.IsWhiteSpace(unparsedParameters[i - 1]))
                {
                    if (nextParameterTypeCode.IsNumeric())
                    {
                        if (char.IsNumber(unparsedParameters[i]))
                            break;
                    }
                    else if (nextParameterTypeCode == TypeCode.Char)
                    {
                        if (i < unparsedParameters.Length - 1 && !char.IsWhiteSpace(unparsedParameters[i + 1]))
                            break;
                    }
                }
                else if (!escape && unparsedParameters[i].Equals('\\'))
                {
                    writeCharacter = false;
                    escape = true;
                }

                if(writeCharacter)
                {
                    escape = false;
                    result += unparsedParameters[i];
                }
                removeCharacters++;
            }

            if (result == string.Empty)
            {
                if (isOptional)
                    return defaultValue;
                else
                    throw new TargetParameterCountException();
            }
            else
            {
                unparsedParameters = unparsedParameters.Remove(0, removeCharacters);

                int i = result.Length - 1;
                while(char.IsWhiteSpace(result[i]))
                    i--;

                if ((i + 1) < result.Length)
                    result = result.Remove(i + 1);

                return result;
            }
        }
    }
}
