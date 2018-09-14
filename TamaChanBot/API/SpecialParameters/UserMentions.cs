using System;

namespace TamaChanBot.API.SpecialParameters
{
    public class UserMentions : MentionParameter
    {
        public override void ParseMessageContext(MessageContext messageContext, bool isOptional, object defaultValue)
        {
            if (messageContext.mentionedUsers.Length > 0)
                this.mentions = messageContext.mentionedUsers;
            else
                throw new System.Reflection.TargetParameterCountException();
        }
    }
}
