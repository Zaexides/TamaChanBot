using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TamaChanBot.Utility
{
    public static class TypeCodeHelper
    {
        public static bool IsNumeric(this TypeCode typeCode)
        {
            int typeCodeIndex = (int)typeCode;
            return typeCodeIndex >= 5 && typeCodeIndex <= 15;
        }
    }
}
