using System;
using System.Collections.Generic;
using System.Reflection;
using TamaChanBot.API;

namespace TamaChanBot.Core
{
    public sealed partial class CommandInvoker
    {
        private Array ParseArrayParameter(ref string unparsedParameters, Type arrayType, bool isOptional, bool nextParameterIsNumeric, MessageContext messageContext)
        {
            TamaChan.Instance.Logger.LogInfo("Test");
            List<object> objectList = new List<object>();
            string unparsedParametersBackup = unparsedParameters;
            try
            {
                while (unparsedParameters != null && unparsedParameters.Length > 0)
                {
                    unparsedParametersBackup = unparsedParameters;
                    object member = GetParameter(ref unparsedParameters, arrayType, false, null, null, messageContext);
                    if (Type.GetTypeCode(arrayType) == TypeCode.Char && nextParameterIsNumeric)
                    {
                        if (char.IsNumber((char)member))
                        {
                            unparsedParameters = unparsedParametersBackup;
                            break;
                        }
                    }
                    objectList.Add(member);
                }
            }
            catch
            {
                unparsedParameters = unparsedParametersBackup;
            }
            finally
            {
                if (objectList.Count == 0)
                {
                    if (isOptional)
                        objectList = null;
                    else
                        throw new TargetParameterCountException();
                }
            }
            if (objectList == null)
                return null;
            object[] abstractArray = objectList.ToArray();
            Array concreteArray = Array.CreateInstance(arrayType, abstractArray.Length);
            Array.Copy(abstractArray, concreteArray, concreteArray.Length);
            return concreteArray;
        }
    }
}
