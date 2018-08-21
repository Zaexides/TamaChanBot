using System;
using TamaChanBot.Core;

namespace TamaChanBot.API.Responses
{
    internal static class EmbedResponseTemplateFactory
    {
        private const int TEMPLATE_SYSTEM_MESSAGE_START = (int)EmbedResponseTemplate.Error;
        private const int TEMPLATE_SYSTEM_MESSAGE_END = (int)EmbedResponseTemplate.Failure;

        private const uint COLOR_DEFAULT = 0x90A2DD;
        private const uint COLOR_YELLOW = 0xECDD00;
        private const uint COLOR_RED = 0xEC1F1F;
        private const uint COLOR_GREEN = 0x41D34D;

        private const string TITLE_INFO = "Info";
        private const string TITLE_WARNING = "Warning";
        private const string TITLE_ERROR = "Error";
        private const string TITLE_SUCCESS = "Success";
        private const string TITLE_FAILURE = "Failure";

        internal static EmbedResponse CreateFromTemplate(EmbedResponseTemplate template)
        {
            EmbedResponse embedResponse = new EmbedResponse();
            if ((int)template >= TEMPLATE_SYSTEM_MESSAGE_START && (int)template <= TEMPLATE_SYSTEM_MESSAGE_END)
            {
                embedResponse.Author = TamaChan.Instance.botSettings.botName;
                if (TamaChan.Instance.GetSelf() == null)
                    embedResponse.IconUrl = "%SELF%";
                else
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
                case EmbedResponseTemplate.Failure:
                    return COLOR_RED;
                case EmbedResponseTemplate.Success:
                    return COLOR_GREEN;
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
                case EmbedResponseTemplate.Info:
                    return TITLE_INFO;
                case EmbedResponseTemplate.Success:
                    return TITLE_SUCCESS;
                case EmbedResponseTemplate.Failure:
                    return TITLE_FAILURE;
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
        Info = 22,
        Success = 23,
        Failure = 24,
    }
}
