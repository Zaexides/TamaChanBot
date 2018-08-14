using System;
using System.Reflection;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private object GetNumericParameter(string unparsedParameter, bool isOptional, object defaultValue, TypeCode typeCode)
        {
            object value;
            try
            {
                value = Convert.ChangeType(unparsedParameter, typeCode);
            }
            catch
            {
                if (isOptional)
                    value = defaultValue;
                else if (unparsedParameter == null || unparsedParameter.Equals(string.Empty))
                    throw new TargetParameterCountException();
                else
                {
                    string laymanTranslation = "number";
                    if (typeCode == TypeCode.Char)
                        laymanTranslation = "character";
                    throw new ArgumentNullException($"\"{unparsedParameter}\" is not a valid {laymanTranslation}.");
                }
            }
            return value;
        }
    }
}
