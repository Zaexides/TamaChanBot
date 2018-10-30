using System;
using System.Collections.Generic;
using System.Linq;

namespace TamaChanBot.API.Responses
{
    public class Menu : MenuHandlerObject<int>
    {
        public readonly string[] options;

        public bool AllowNumericAnswers { get; set; } = true;
        public bool CancelOnNonOption { get; set; } = true;
        
        public Menu(string title, ResponseHandler<int> responseHandler, params string[] options) : this(title, "", responseHandler, options)
        {
        }
        
        public Menu(string title, string description, ResponseHandler<int> responseHandler, params string[] options) : base(title, responseHandler, description)
        {
            if (options == null || options.Length == 0)
                throw new ArgumentException("Parameter array needs to have 1 or more items.", "options");

            this.options = options.Distinct().ToArray();

            this.responseHandler = responseHandler;
        }

        public static Menu CreateMenu<TEnum>(string title, ResponseHandler<int> responseHandler) where TEnum : Enum
            => CreateMenu<TEnum>(title, "", responseHandler);

        public static Menu CreateMenu<TEnum>(string title, string description, ResponseHandler<int> responseHandler) where TEnum:Enum
        {
            string[] stringOptions = Enum.GetNames(typeof(TEnum)).Select(e => e.Replace('_', ' ')).ToArray();
            return new Menu(title, description, responseHandler, stringOptions);
        }

        public override bool HandleMenuOperation(out object response, MessageContext messageContext)
        {
            int optionId;
            string[] lowercaseOptions = options.Select(s => s.ToLowerInvariant()).ToArray();
            if (lowercaseOptions.Contains(messageContext.message.ToLower()))
            {
                optionId = lowercaseOptions.ToList().FindIndex(o => o.Equals(messageContext.message.ToLower()));

                response = this.responseHandler(optionId, messageContext, data);
                return true;
            }
            else if (AllowNumericAnswers && NumericAnswer(messageContext, options.Length, out optionId))
            {
                response = this.responseHandler(optionId, messageContext, data);
                return true;
            }
            else if (CancelOnNonOption)
            {
                response = this.responseHandler(-1, messageContext, data);
                return true;
            }
            else
            {
                response = null;
                return false;
            }
        }

        private bool NumericAnswer(MessageContext messageContext, int maxOptions, out int optionId)
        {
            if (int.TryParse(messageContext.message, out optionId))
            {
                optionId--;
                return optionId > 0 && optionId < maxOptions;
            }
            return false;
        }
    }
}
