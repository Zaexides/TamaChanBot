using System;

namespace TamaChanBot.API.SpecialParameters
{
    public class RoleMentions : MentionParameter
    {
        public override void ParseMessageContext(MessageContext messageContext, bool isOptional, object defaultValue)
        {
            if (messageContext.mentionedRoles.Length > 0)
                this.mentions = messageContext.mentionedRoles;
            else
                throw new System.Reflection.TargetParameterCountException();
        }
    }
}
