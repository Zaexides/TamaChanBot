using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TamaChanBot.API
{
    public struct URL : IParameterParser
    {
        public Uri uri;

        public void CopyFrom(URL url)
        {
            this.uri = url.uri;
        }

        public void ParseParameter(ref string remainingParameters, bool isOptional, object defaultValue, ParameterInfo nextParameter)
        {
            string word = remainingParameters.ToLower();
            int nextSpace = word.IndexOf(' ');
            if (nextSpace > 0)
                word = word.Substring(0, nextSpace);
            bool s;
            if (s = Uri.TryCreate(word, UriKind.Absolute, out uri))
            {
                if (remainingParameters.Length > word.Length)
                    remainingParameters = remainingParameters.Remove(0, word.Length + 1);
                else
                    remainingParameters = string.Empty;
            }
            else if (isOptional)
            {
                if (defaultValue != null)
                    CopyFrom((URL)defaultValue);
            }
            else
                throw new TargetParameterCountException();
        }
    }
}
