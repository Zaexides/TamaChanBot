using System;
using System.Reflection;

namespace TamaChanBot.API
{
    public interface IParameterParser
    {
        void ParseParameter(ref string remainingParameters, bool isOptional, object defaultValue, ParameterInfo nextParameter);
    }
}
