using System;

namespace TamaChanBot.API.Responses
{
    public class MenuResponse : BaseMenuResponse
    {
        public MenuResponse(Menu menu, MessageContext context) : base(menu, context)
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

            this.messages = new Message[1]
            {
                new Message("Options:", string.Join("\r\n", options))
            };
        }
    }
}
