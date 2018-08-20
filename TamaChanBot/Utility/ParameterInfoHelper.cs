using System;
using System.Reflection;

namespace TamaChanBot.Utility
{
    public static class ParameterInfoHelper
    {
        public static bool IsOptionalParameter(this ParameterInfo parameterInfo, ParameterInfo nextParameter = null)
        {
            bool isOptional = parameterInfo.HasDefaultValue || parameterInfo.IsOptional;
            if (nextParameter == null)
                isOptional = isOptional || parameterInfo.GetCustomAttribute<ParamArrayAttribute>() != null;
            return isOptional;
        }
    }
}
