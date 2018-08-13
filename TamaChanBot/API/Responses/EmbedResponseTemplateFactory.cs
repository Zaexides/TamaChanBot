using System;
using TamaChanBot.Core;

namespace TamaChanBot.API.Responses
{
    internal static class EmbedResponseTemplateFactory
    {
        private const int TEMPLATE_SYSTEM_MESSAGE_START = (int)EmbedResponseTemplate.Error;
        private const int TEMPLATE_SYSTEM_MESSAGE_END = (int)EmbedResponseTemplate.Warning;

        private const uint COLOR_DEFAULT = 0x656C8E;
        private const uint COLOR_YELLOW = 0xECDD00;
        private const uint COLOR_RED = 0xEC1F1F;

        private const string TITLE_WARNING = "Warning";
        private const string TITLE_ERROR = "Error";

        internal static EmbedResponse CreateFromTemplate(EmbedResponseTemplate template)
        {
            EmbedResponse embedResponse = new EmbedResponse();
            if ((int)template >= TEMPLATE_SYSTEM_MESSAGE_START && (int)template <= TEMPLATE_SYSTEM_MESSAGE_END)
            {
                embedResponse.Author = TamaChan.Instance.botSettings.botName;
                embedResponse.IconUrl = TamaChan.Instance.GetSelf().GetAvatarUrl(Discord.ImageFormat.Auto, 64);
            }
            embedResponse.Title = GetTemplateTitle(template);
            embedResponse.Color = GetTemplateColor(template);
            return embedResponse;
        }

        private static uint GetTemplateColor(EmbedResponseTemplate template)
        {
            switch(template)
            {
                case EmbedResponseTemplate.Warning:
                    return COLOR_YELLOW;
                case EmbedResponseTemplate.Error:
                    return COLOR_RED;
                default:
                    return COLOR_DEFAULT;
            }
        }

        private static string GetTemplateTitle(EmbedResponseTemplate template)
        {
            switch(template)
            {
                case EmbedResponseTemplate.Warning:
                    return TITLE_WARNING;
                case EmbedResponseTemplate.Error:
                    return TITLE_ERROR;
                default:
                    return string.Empty;
            }
        }
    }

    public enum EmbedResponseTemplate
    {
        Empty = 0,

        Error = 20,
        Warning = 21,
    }
}
