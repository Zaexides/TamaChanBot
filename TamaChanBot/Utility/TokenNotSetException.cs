using System;

namespace TamaChanBot.Utility
{
    [Serializable]
    public class TokenNotSetException : Exception
    {
        public TokenNotSetException() { }
        public TokenNotSetException(string message) : base(message) { }
        public TokenNotSetException(string message, Exception inner) : base(message, inner) { }
        protected TokenNotSetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
