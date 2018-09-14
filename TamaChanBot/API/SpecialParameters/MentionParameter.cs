using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TamaChanBot.API.SpecialParameters
{
    public abstract class MentionParameter : IMessageContextParser
    {
        protected MessageContext.MentionInfo[] mentions;

        public MessageContext.MentionInfo this[int index] => mentions[index];
        public int Length => mentions.Length;

        public abstract void ParseMessageContext(MessageContext messageContext, bool isOptional, object defaultValue);
    }
}
