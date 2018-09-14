using System;

namespace TamaChanBot.API.SpecialParameters
{
    public class ChannelMentions : MentionParameter
    {
        public override void ParseMessageContext(MessageContext messageContext, bool isOptional, object defaultValue)
        {
            if (messageContext.mentionedChannels.Length > 0)
                this.mentions = messageContext.mentionedChannels;
            else
                throw new System.Reflection.TargetParameterCountException();
        }
    }
}
