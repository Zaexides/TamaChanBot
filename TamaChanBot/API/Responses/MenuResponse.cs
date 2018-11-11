using System;
using System.Collections.Generic;

namespace TamaChanBot.API.Responses
{
    public class MenuResponse : BaseMenuResponse
    {
        public MenuResponse(Menu menu, MessageContext context) : base(menu, context)
        {
            AddOptions(menu);
        }

        public MenuResponse(Menu menu, MessageContext context, EmbedResponse baseEmbed) : base(menu, context, baseEmbed)
        {
            AddOptions(menu);
        }

        private void AddOptions(Menu menu)
        {
            string[] options;
            if (menu.AllowNumericAnswers)
            {
                options = new string[menu.options.Length];
                for (int i = 0; i < menu.options.Length; i++)
                    options[i] = $"{i + 1} - {menu.options[i]}";
            }
            else
                options = menu.options;

            List<Message> messages;
            if (this.messages != null)
                messages = new List<Message>(this.messages);
            else
                messages = new List<Message>();
            messages.Add(new Message("Options:", string.Join("\r\n", options)));
            this.messages = messages.ToArray();
        }
    }
}
