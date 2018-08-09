using System;
using System.Text.RegularExpressions;

namespace TamaChanBot.Utility
{
    public static class StringHelper
    {
        public static readonly Regex registryValidatorRegex = new Regex(@"[^a-zA-Z.]");

        public static bool IsRegistryValid(this string input) => !registryValidatorRegex.IsMatch(input);
    }
}
